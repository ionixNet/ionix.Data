namespace ionix.Data
{
    using System;

    [Serializable]
    public enum StoreGeneratedPattern : int //DatabaseGeneratedOption
    {
        None = 0,//Guid, Manuel Sequence Value. (insert list de olacak)
        Identity = 1,//Identity Column. (insert list de olmayacak). Kısıt olarak IEntityMetaData.Properties de mutlaka tekil olmalı.
        Computed = 2,//Column with Default Value(i.e getdate(), deleted 0,), Guid as DefaultValue, Next Sequence Value as Default Value.
                     //  Sequence = 3,//TKADRO da Seqeunce insert liste eklenmeli ve values liste de yazılmalı.  sequence check edilmeden yani Sequence İdentity gibi default value ile verilmeli
                     //Sequence Yerine none veya Computed Kullan
        AutoGenerateSequence = 3//Classical Oracle Sequence and returning next value like identity. Kısıt olarak IEntityMetaData.Properties de mutlaka tekil olmalı.
    }

    [Serializable]
    public enum SqlValueType : int
    {
        Parameterized = 0,
        Text
    }

    //Key LER EntityFramework de boolean bir alanda tutuluyor DatabaseGeneratedOption ayrı veriliyor.
}
/*
Computed	A value is generated on both insert and update.
Identity	A value is generated on insert and remains unchanged on update.
None	A value indicating that it is not a server generated property. This is the default value. 
 */


/*
 
    //Örneğin DefaultValue DB den verildiyse ekleme yapılmaması için.
    //Veya Identity Column
    public sealed class InsertableAttribute : Attribute
    {
        private readonly bool allowInsert;
        public InsertableAttribute(bool allowInsert)
        {
            this.allowInsert = allowInsert;
        }
        public bool AllowInsert
        {
            get { return this.allowInsert; }
        }
    }
 * 
 *  public sealed class IdentityAttribute : Attribute
    {

    }

 */
