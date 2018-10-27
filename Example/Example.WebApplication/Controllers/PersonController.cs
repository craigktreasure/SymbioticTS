using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Example.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        // GET: api/Person
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            yield return this.GetPerson();
        }

        // GET: api/Person/5
        [HttpGet("{id}", Name = "Get")]
        public Person Get(int id)
        {
            return this.GetPerson();
        }

        private Person GetPerson()
        {
            return new Person
            {
                Age = 42,
                BirthDate = DateTime.Now.AddYears(-30),
                FirstName = "John",
                Gender = Gender.Male,
                LastName = "Jackson"
            };
        }
    }
}