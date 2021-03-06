﻿using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Virgo.Extensions;
using Xunit;

namespace Virgo.Tests.Extensions
{
    public class JsonExtensions_Test
    {
        [Fact]
        public void Simple_Serialize_Test()
        {
            var person = new Person("Virgo");
            var json = person.Serialize();
            json.ShouldBe("{\"Name\":\"Virgo\"}");
        }
        [Fact]
        public void Simple_Deserialize_Test()
        {
            var json = "{\"Name\":\"Virgo\"}";
            var person = json.Deserialize<Person>();
            person.Name.ShouldBe("Virgo");
        }

    }

    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public Person()
        {
        }
        public Person(string name)
        {
            Name = name;
        }
    }
}
