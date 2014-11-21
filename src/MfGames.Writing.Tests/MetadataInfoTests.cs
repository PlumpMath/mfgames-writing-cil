// <copyright file="MetadataInfoTests.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Tests
{
    using MfGames.Writing.Markdown;

    using NUnit.Framework;

    /// <summary>
    /// </summary>
    [TestFixture]
    public class MetadataInfoTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        [Test]
        public void CopryightSingleYear()
        {
            var metadata = new MetadataInfo();
            metadata.Add("Copyright", "2014, Dylan Moonfire");
            Assert.AreEqual(
                1, 
                metadata.CopyrightYears.Count, 
                "CopyrightYears.Count is unexpected.");
            Assert.AreEqual(
                2014, 
                metadata.CopyrightYears[0], 
                "CopyrightYears[0] is unexpected.");
            Assert.AreEqual(
                "Dylan Moonfire", 
                metadata.CopyrightHolder, 
                "CopyrightHolder is unexpected.");
        }

        /// <summary>
        /// </summary>
        [Test]
        public void CopyrightDoubleYears()
        {
            var metadata = new MetadataInfo();
            metadata.Add("Copyright", "2014, 2015, Dylan Moonfire");
            Assert.AreEqual(
                2, 
                metadata.CopyrightYears.Count, 
                "CopyrightYears.Count is unexpected.");
            Assert.AreEqual(
                2014, 
                metadata.CopyrightYears[0], 
                "CopyrightYears[0] is unexpected.");
            Assert.AreEqual(
                2015, 
                metadata.CopyrightYears[1], 
                "CopyrightYears[1] is unexpected.");
            Assert.AreEqual(
                "Dylan Moonfire", 
                metadata.CopyrightHolder, 
                "CopyrightHolder is unexpected.");
        }

        /// <summary>
        /// </summary>
        [Test]
        public void Date()
        {
            var metadata = new MetadataInfo();
            metadata.Add("date", "2014-10-12");
            Assert.IsTrue(metadata.Date.HasValue, "Date is unexpected.");
            Assert.AreEqual(
                2014, metadata.Date.Value.Year, "Date.Year is unexpected.");
            Assert.AreEqual(
                10, metadata.Date.Value.Month, "Date.Month is unexpected.");
            Assert.AreEqual(
                12, metadata.Date.Value.Day, "Date.Day is unexpected.");
        }

        /// <summary>
        /// </summary>
        [Test]
        public void SchemesCommaSeparated()
        {
            var metadata = new MetadataInfo();
            metadata.Add("Bob", "gary, steve");
            Assert.AreEqual(
                1, metadata.Schemes.Count, "Schemes.Count is unexpected.");
            Assert.AreEqual(
                2, 
                metadata.Schemes["Bob"].Count, 
                "Schemes[Bob].Count is unexpected.");
            Assert.AreEqual(
                "gary", 
                metadata.Schemes["Bob"][0], 
                "Schemes[Bob][0] is unexpected.");
            Assert.AreEqual(
                "steve", 
                metadata.Schemes["Bob"][1], 
                "Schemes[Bob][1] is unexpected.");
        }

        /// <summary>
        /// </summary>
        [Test]
        public void SchemesSemicolonSeparated()
        {
            var metadata = new MetadataInfo();
            metadata.Add("Bob", "gary; steve");
            Assert.AreEqual(
                1, metadata.Schemes.Count, "Schemes.Count is unexpected.");
            Assert.AreEqual(
                2, 
                metadata.Schemes["Bob"].Count, 
                "Schemes[Bob].Count is unexpected.");
            Assert.AreEqual(
                "gary", 
                metadata.Schemes["Bob"][0], 
                "Schemes[Bob][0] is unexpected.");
            Assert.AreEqual(
                "steve", 
                metadata.Schemes["Bob"][1], 
                "Schemes[Bob][1] is unexpected.");
        }

        /// <summary>
        /// </summary>
        [Test]
        public void SchemesSemicolonWithCommas()
        {
            var metadata = new MetadataInfo();
            metadata.Add("Bob", "gary; steve, larry");
            Assert.AreEqual(
                1, metadata.Schemes.Count, "Schemes.Count is unexpected.");
            Assert.AreEqual(
                2, 
                metadata.Schemes["Bob"].Count, 
                "Schemes[Bob].Count is unexpected.");
            Assert.AreEqual(
                "gary", 
                metadata.Schemes["Bob"][0], 
                "Schemes[Bob][0] is unexpected.");
            Assert.AreEqual(
                "steve, larry", 
                metadata.Schemes["Bob"][1], 
                "Schemes[Bob][1] is unexpected.");
        }

        /// <summary>
        /// </summary>
        [Test]
        public void SchemesSingleKeyValue()
        {
            var metadata = new MetadataInfo();
            metadata.Add("Bob", "gary");
            Assert.AreEqual(
                1, metadata.Schemes.Count, "Schemes.Count is unexpected.");
            Assert.AreEqual(
                1, 
                metadata.Schemes["Bob"].Count, 
                "Schemes[Bob].Count is unexpected.");
            Assert.AreEqual(
                "gary", 
                metadata.Schemes["Bob"][0], 
                "Schemes[Bob][0] is unexpected.");
        }

        /// <summary>
        /// </summary>
        [Test]
        public void Title()
        {
            var metadata = new MetadataInfo();
            metadata.Add("Title", "Simple");
            Assert.AreEqual("Simple", metadata.Title);
        }

        #endregion
    }
}