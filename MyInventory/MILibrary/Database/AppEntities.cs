namespace MILibrary.Database.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class MI_HISTORY
    {
        [Key]
        public int HISTORY_ID { get; set; }

        public DateTime UPDATEDTTM { get; set; }

        [Required]
        [StringLength(100)]
        public string TABLENAME { get; set; }

        public int IDENTIFIER { get; set; }

        [Required]
        [StringLength(1000)]
        public string ACTION { get; set; }
    }

    public partial class MI_STATUS_REF
    {
        public MI_STATUS_REF()
        {
            MI_USER = new HashSet<MI_USER>();
            MI_WAREHOUSE = new HashSet<MI_WAREHOUSE>();
            MI_WH_ITEM = new HashSet<MI_WH_ITEM>();
            MI_WH_TRANSACTION = new HashSet<MI_WH_TRANSACTION>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int STATUS_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string STATUS { get; set; }

        public virtual ICollection<MI_USER> MI_USER { get; set; }

        public virtual ICollection<MI_WAREHOUSE> MI_WAREHOUSE { get; set; }

        public virtual ICollection<MI_WH_ITEM> MI_WH_ITEM { get; set; }

        public virtual ICollection<MI_WH_TRANSACTION> MI_WH_TRANSACTION { get; set; }
    }

    public partial class MI_TRANSACTIONTYPE_REF
    {
        public MI_TRANSACTIONTYPE_REF()
        {
            MI_WH_TRANSACTION = new HashSet<MI_WH_TRANSACTION>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TRANSACTIONTYPE_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string TRANSACTIONTYPE { get; set; }

        [StringLength(10)]
        public string ACTION { get; set; }

        public virtual ICollection<MI_WH_TRANSACTION> MI_WH_TRANSACTION { get; set; }
    }

    public partial class MI_USER
    {
        public MI_USER()
        {
            MI_WAREHOUSE = new HashSet<MI_WAREHOUSE>();
        }

        [Key]
        public int USER_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string FIRSTNAME { get; set; }

        [Required]
        [StringLength(150)]
        public string LASTNAME { get; set; }

        [Required]
        [StringLength(150)]
        public string EMAIL { get; set; }

        public Guid SALT { get; set; }

        [Required]
        [MaxLength(64)]
        public byte[] PASSWORD_HASH { get; set; }

        public DateTime CREATEDDTTM { get; set; }

        public int STATUS_ID { get; set; }

        public virtual MI_STATUS_REF MI_STATUS_REF { get; set; }

        public virtual ICollection<MI_WAREHOUSE> MI_WAREHOUSE { get; set; }
    }

    public partial class MI_WAREHOUSE
    {
        public MI_WAREHOUSE()
        {
            MI_WH_ITEM = new HashSet<MI_WH_ITEM>();
        }

        [Key]
        public int WAREHOUSE_ID { get; set; }

        public int USER_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string NAME { get; set; }

        [StringLength(250)]
        public string DESCRIPTION { get; set; }

        public int STATUS_ID { get; set; }

        public virtual MI_STATUS_REF MI_STATUS_REF { get; set; }

        public virtual MI_USER MI_USER { get; set; }

        public virtual ICollection<MI_WH_ITEM> MI_WH_ITEM { get; set; }
    }

    public partial class MI_WH_ITEM
    {
        public MI_WH_ITEM()
        {
            MI_WH_TRANSACTION = new HashSet<MI_WH_TRANSACTION>();
        }

        [Key]
        public int ITEM_ID { get; set; }

        public int WAREHOUSE_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string NAME { get; set; }

        [StringLength(250)]
        public string DESCRIPTION { get; set; }

        [Column(TypeName = "money")]
        public decimal? PRICE { get; set; }

        public int QUANTITY { get; set; }

        [StringLength(20)]
        public string UOM { get; set; }

        public int STATUS_ID { get; set; }

        public virtual MI_STATUS_REF MI_STATUS_REF { get; set; }

        public virtual MI_WAREHOUSE MI_WAREHOUSE { get; set; }

        public virtual ICollection<MI_WH_TRANSACTION> MI_WH_TRANSACTION { get; set; }
    }

    public partial class MI_WH_TRANSACTION
    {
        [Key]
        public int TRANSACTION_ID { get; set; }

        public int TRANSACTIONTYPE_ID { get; set; }

        public int ITEM_ID { get; set; }

        public int TRANSACTION_AMOUNT { get; set; }

        public int NEW_AMOUNT { get; set; }

        public int STATUS_ID { get; set; }

        public DateTime TRANSACTIONDTTM { get; set; }

        public virtual MI_STATUS_REF MI_STATUS_REF { get; set; }

        public virtual MI_TRANSACTIONTYPE_REF MI_TRANSACTIONTYPE_REF { get; set; }

        public virtual MI_WH_ITEM MI_WH_ITEM { get; set; }
    }
}
