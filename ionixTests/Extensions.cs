using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ionix.Utils.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ionixTests
{
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
