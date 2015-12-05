﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Tests
{
	[TestClass]
	public class BelpostSearcherTests
	{
		[TestMethod]
		public void Search_Should_Find()
		{
			IUpdateSearcher searcher = new BelpostSearcher();
			var result = searcher.Search(new PackageDTO {Tracking = "RM611628067CN" });
            Assert.IsNotNull(result);
			Assert.IsTrue(result.Contains("Доставлено, вручено"));
		}
	}
}