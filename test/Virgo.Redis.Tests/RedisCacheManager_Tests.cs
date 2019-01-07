﻿using Autofac.Extras.IocManager;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Proxy;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Virgo.Cache;
using Virgo.Cache.Configuration;
using Xunit;

namespace Virgo.Redis.Tests
{
    public class RedisCacheManager_Tests
    {
        private readonly ICacheManager _cacheManager;
        private readonly ITypedCache<string, MyCacheItem> _cache;
        public RedisCacheManager_Tests()
        {
            IRootResolver resolver = IocBuilder.New
                                      .UseAutofacContainerBuilder()
                                      .RegisterServices(r =>
                                      {
                                          r.Register(typeof(ICachingConfiguration), typeof(CachingConfiguration), Lifetime.Singleton);
                                          r.Register(typeof(IRedisCaCheConfiguration), typeof(RedisCaCheConfiguration), Lifetime.Singleton);
                                          r.Register(typeof(IRedisCacheProvider), typeof(RedisCacheProvider), Lifetime.Singleton);
                                          //r.Register(typeof(ICache), typeof(RedisCache),Lifetime.LifetimeScope);
                                          r.Register(typeof(ICacheManager), typeof(RedisCacheManager), Lifetime.Singleton); 
                                      })
                                      .RegisterIocManager()
                                      .CreateResolver()
                                      .UseIocManager();
            resolver.Resolve<IRedisCaCheConfiguration>().DatabaseId=0;
            _cacheManager = resolver.Resolve<ICacheManager>();
            resolver.Resolve<ICachingConfiguration>().ConfigureAll(cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromHours(24);
            });            
            _cache = _cacheManager.GetCache<string, MyCacheItem>("MyCacheItems");
        }

        [Fact]
        public void Simple_Get_Set_Test()
        {
            _cache.GetOrDefault("A").ShouldBe(null);
            _cache.Set("A", new MyCacheItem { Value = 42 });
            _cache.GetOrDefault("A").ShouldNotBe(null);
            _cache.GetOrDefault("A").Value.ShouldBe(42);

            _cache.Get("B", () => new MyCacheItem { Value = 43 }).Value.ShouldBe(43);
            _cache.Get("B", () => new MyCacheItem { Value = 44 }).Value.ShouldBe(43); //不调用工厂，所以值不变

            var items1 = _cache.GetOrDefault(new string[] { "B", "C" });
            items1[0].Value.ShouldBe(43);
            items1[1].ShouldBeNull();

            var items2 = _cache.GetOrDefault(new string[] { "C", "D" });
            items2[0].ShouldBeNull();
            items2[1].ShouldBeNull();

            _cache.Set(new KeyValuePair<string, MyCacheItem>[] {
                new KeyValuePair<string, MyCacheItem>("C", new MyCacheItem{ Value = 44}),
                new KeyValuePair<string, MyCacheItem>("D", new MyCacheItem{ Value = 45})
            });

            var items3 = _cache.GetOrDefault(new string[] { "C", "D" });
            items3[0].Value.ShouldBe(44);
            items3[1].Value.ShouldBe(45);

            var items4 = _cache.Get(new string[] { "D", "E" }, (key) => new MyCacheItem { Value = key == "D" ? 46 : 47 });
            items4[0].Value.ShouldBe(45); //不调用工厂，所以值不变
            items4[1].Value.ShouldBe(47);
        }
    }
    [Serializable]
    public class MyCacheItem
    {
        public int Value { get; set; }

        public MyCacheItem()
        {

        }

        public MyCacheItem(int value)
        {
            Value = value;
        }
    }
}