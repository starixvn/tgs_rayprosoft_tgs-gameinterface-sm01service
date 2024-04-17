namespace Tgs.GameInterface.SM01Service.Core.UnitTest;

[TestClass]
public class HasherUnitTests
{
	[TestMethod]
	public void Sign_Test()
	{
		var dict = new Dictionary<string, string>
		{
			{ "merchantId","SQjCxm2cX5bahBta" },
			{ "timestamp", "1670390195123"},
			{ "customerName","4ucmqplxb029"}
		};
		var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";

		var actual = SM01Hasher.ComputeHash(merchantKey, dict);

		Assert.AreEqual("46C9EA4F03900B74379A15A56ED14229", actual);
	}

	[TestMethod]
	public void Sign_Test1()
	{
		var dict = new Dictionary<string, string>
		{
			{ "merchantId","SQjCxm2cX5bahBta" },
			{ "timestamp", "1670410701621"},
			{ "customerName","4ucmqplxb029"},
			{ "StartDate","2022-12-07 00:04:09"},
			{ "EndDate","2022-12-07 18:04:09"},
			{ "type", "1" }
		};
		var merchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb";

		var actual = SM01Hasher.ComputeHash(merchantKey, dict);

		Assert.AreEqual("87518AEC98AB2F0A03E8B175025E299F", actual);
	}
}
