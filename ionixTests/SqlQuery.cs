using System;//
using System.Diagnostics;
using ionix.Data;
using ionix.Utils;
using ionix.Utils.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ionixTests
{
    [TestClass]
    public class SqlQuerytTest
    {
        [TestMethod]
        public void XmlTest()
        {
            SqlQuery q = @"SELECT M.OID,N.NSN,N.NSC,N.NIIN, N.FIIG_4065,N.NCB,N.INC_2303, N.MALZEME_ADI, M.TALEP_NO 
            FROM MSB.MSB_YRL_NSN N, MSB.MSB_YRL_TLP_TALEP_NSN_MLZ M 
            WHERE M.NSN = N.NSN AND M.TALEP_OID = '0000000000008663' 
            ORDER BY NIIN ASC".ToQuery(12.42M, 112, "dsadsa", 12.0, Guid.NewGuid());


            string xml = q.XmlSerialize();

            var copy = q.Copy();

            Stopwatch bench = Stopwatch.StartNew();
            for (int j = 0; j < 100; ++j)
                copy = q.Copy();
            bench.Stop();

            Debug.WriteLine("Sql Query Xml Copy: " + bench.ElapsedMilliseconds);

            Assert.IsNotNull(copy);
        }
    }
}
