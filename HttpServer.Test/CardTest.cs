using NUnit.Framework;
using System.Linq;
using HttpServer;
using HttpServer.Models;

namespace HttpServer.Test
{
    [TestFixture]
    class CardTest
    {
        [Test]
        public void CreateMonsterCardAndCheckName()
        {
            //arrange
            var monster = new Monster("kevin12", "Dragon", 20);

            //act
            var returnedName = monster.GetNameWithDamage();

            //assert
            Assert.AreEqual("NormalDragon(20)", returnedName);
        }

        [Test]
        public void CreateSpellCardAndCheckName()
        {
            //arrange
            var spell = new Spell(null, "FireSpell", 20);

            //act
            var returnedName = spell.GetNameWithDamage();

            //assert
            Assert.AreEqual("FireSpell(20)", returnedName);
        }

        //[Test]
        //public void CreatePackage()
        //{
        //    //arrange
        //    //act
        //    var package = PackageGenerator.GeneratePackage();

        //    //assert
        //    Assert.AreEqual(5, package.Count());
        //}

    }
}