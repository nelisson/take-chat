using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeChat.Domain.Entities;
using Xunit;

namespace TakeChat.Domain.Test
{
    public class UserTest
    {
        private const string MAXIMUM_CHAR_NAME = "Lorem ipsum dolor sit amet, consectetur tincidunt.";

        [Fact]
        public void Constructor_WithEmptyName_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new User(""));
        }

        [Fact]
        public void Constructor_WithBodyExcedingMaxCharsAllowed_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new User(MAXIMUM_CHAR_NAME + "A"));
        }

        [Fact]
        public void Constructor_WithCorrectParams_CanGetSameValues()
        {
            var username = Faker.Internet.UserName();
            
            var user = new User(username);

            Assert.Equal(username, user.Name);
        }
    }
}
