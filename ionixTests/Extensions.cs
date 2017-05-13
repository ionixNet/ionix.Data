namespace ionixTests
{
    using ionix.Utils.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void IfNullSetEmptyTest()
        {
            object o = "dsadas";

            object value = o.IfNullSetEmpty<string>();

            Assert.IsNotNull(value);

        }
    }
}
