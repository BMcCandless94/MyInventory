namespace MILibrary.Database
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using MILibrary.Database.Entities;

    public partial class AppDbContext : DbContext
    {
        public AppDbContext(string ConnectionString)
            :base(ConnectionString)
        {
        }
        public static AppDbContext Create(string ConnectionString) => new AppDbContext(ConnectionString);

        //Database Entities
        public virtual DbSet<MI_HISTORY> MI_HISTORY { get; set; }
        public virtual DbSet<MI_STATUS_REF> MI_STATUS_REF { get; set; }
        public virtual DbSet<MI_TRANSACTIONTYPE_REF> MI_TRANSACTIONTYPE_REF { get; set; }
        public virtual DbSet<MI_USER> MI_USER { get; set; }
        public virtual DbSet<MI_WAREHOUSE> MI_WAREHOUSE { get; set; }
        public virtual DbSet<MI_WH_ITEM> MI_WH_ITEM { get; set; }
        public virtual DbSet<MI_WH_TRANSACTION> MI_WH_TRANSACTION { get; set; }

        //Necessary for mapping the relationships between the entities
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MI_STATUS_REF>()
                .HasMany(e => e.MI_USER)
                .WithRequired(e => e.MI_STATUS_REF)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MI_STATUS_REF>()
                .HasMany(e => e.MI_WAREHOUSE)
                .WithRequired(e => e.MI_STATUS_REF)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MI_STATUS_REF>()
                .HasMany(e => e.MI_WH_ITEM)
                .WithRequired(e => e.MI_STATUS_REF)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MI_STATUS_REF>()
                .HasMany(e => e.MI_WH_TRANSACTION)
                .WithRequired(e => e.MI_STATUS_REF)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MI_TRANSACTIONTYPE_REF>()
                .HasMany(e => e.MI_WH_TRANSACTION)
                .WithRequired(e => e.MI_TRANSACTIONTYPE_REF)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MI_USER>()
                .Property(e => e.PASSWORD_HASH)
                .IsFixedLength();

            modelBuilder.Entity<MI_USER>()
                .HasMany(e => e.MI_WAREHOUSE)
                .WithRequired(e => e.MI_USER)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MI_WAREHOUSE>()
                .HasMany(e => e.MI_WH_ITEM)
                .WithRequired(e => e.MI_WAREHOUSE)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MI_WH_ITEM>()
                .Property(e => e.PRICE)
                .HasPrecision(19, 4);

            modelBuilder.Entity<MI_WH_ITEM>()
                .HasMany(e => e.MI_WH_TRANSACTION)
                .WithRequired(e => e.MI_WH_ITEM)
                .WillCascadeOnDelete(false);
        }

        //User Stored Procedures
        internal virtual int UserInsert(MI_USER User, string Password)
        {
            //Set up the parameters
            SqlParameter pFirstName = new SqlParameter
            {
                ParameterName = "pFirstName",
                Value = User.FIRSTNAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            SqlParameter pLastName = new SqlParameter
            {
                ParameterName = "pLastName",
                Value = User.LASTNAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 150,
                Direction = ParameterDirection.Input
            };

            SqlParameter pEmail = new SqlParameter
            {
                ParameterName = "pEmail",
                Value = User.EMAIL,
                SqlDbType = SqlDbType.NVarChar,
                Size = 150,
                Direction = ParameterDirection.Input
            };

            SqlParameter pPassword = new SqlParameter
            {
                ParameterName = "pPassword",
                Value = Password,
                SqlDbType = SqlDbType.NVarChar,
                Size = 30,
                Direction = ParameterDirection.Input
            };

            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIUserInsert @pFirstName, @pLastName, @pEmail, @pPassword, @pUserID OUTPUT",
                new object[] { pFirstName, pLastName, pEmail, pPassword, pUserID });

            //Set the new ID on the object and return it
            User.USER_ID = Convert.ToInt32(pUserID.Value);
            return Convert.ToInt32(pUserID.Value);
        }
        internal virtual async Task<int> UserInsertAsync(MI_USER User, string Password)
        {
            //Set up the parameters
            SqlParameter pFirstName = new SqlParameter
            {
                ParameterName = "pFirstName",
                Value = User.FIRSTNAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            SqlParameter pLastName = new SqlParameter
            {
                ParameterName = "pLastName",
                Value = User.LASTNAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 150,
                Direction = ParameterDirection.Input
            };

            SqlParameter pEmail = new SqlParameter
            {
                ParameterName = "pEmail",
                Value = User.EMAIL,
                SqlDbType = SqlDbType.NVarChar,
                Size = 150,
                Direction = ParameterDirection.Input
            };

            SqlParameter pPassword = new SqlParameter
            {
                ParameterName = "pPassword",
                Value = Password,
                SqlDbType = SqlDbType.NVarChar,
                Size = 30,
                Direction = ParameterDirection.Input
            };

            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIUserInsert @pFirstName, @pLastName, @pEmail, @pPassword, @pUserID OUTPUT",
                new object[] { pFirstName, pLastName, pEmail, pPassword, pUserID });

            //Set the new ID on the object and return it
            User.USER_ID = Convert.ToInt32(pUserID.Value);
            return Convert.ToInt32(pUserID.Value);
        }

        internal virtual void UserUpdateData(MI_USER User)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = User.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pFirstName = new SqlParameter
            {
                ParameterName = "pFirstName",
                Value = User.FIRSTNAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            SqlParameter pLastName = new SqlParameter
            {
                ParameterName = "pLastName",
                Value = User.LASTNAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 150,
                Direction = ParameterDirection.Input
            };

            SqlParameter pEmail = new SqlParameter
            {
                ParameterName = "pEmail",
                Value = User.EMAIL,
                SqlDbType = SqlDbType.NVarChar,
                Size = 150,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIUserUpdateData @pUserID, @pFirstName, @pLastName, @pEmail",
                new object[] { pUserID, pFirstName, pLastName, pEmail});
        }
        internal virtual async Task UserUpdateDataAsync(MI_USER User)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = User.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pFirstName = new SqlParameter
            {
                ParameterName = "pFirstName",
                Value = User.FIRSTNAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            SqlParameter pLastName = new SqlParameter
            {
                ParameterName = "pLastName",
                Value = User.LASTNAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 150,
                Direction = ParameterDirection.Input
            };

            SqlParameter pEmail = new SqlParameter
            {
                ParameterName = "pEmail",
                Value = User.EMAIL,
                SqlDbType = SqlDbType.NVarChar,
                Size = 150,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIUserUpdateData @pUserID, @pFirstName, @pLastName, @pEmail",
                new object[] { pUserID, pFirstName, pLastName, pEmail });
        }

        internal virtual void UserUpdatePassword(MI_USER User, string Password)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = User.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pPassword = new SqlParameter
            {
                ParameterName = "pPassword",
                Value = Password,
                SqlDbType = SqlDbType.NVarChar,
                Size = 30,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIUserUpdatePassword @pUserID, @pPassword",
                new object[] { pUserID, pPassword });
        }
        internal virtual async Task UserUpdatePasswordAsync(MI_USER User, string Password)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = User.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pPassword = new SqlParameter
            {
                ParameterName = "pPassword",
                Value = Password,
                SqlDbType = SqlDbType.NVarChar,
                Size = 30,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIUserUpdatePassword @pUserID, @pPassword",
                new object[] { pUserID, pPassword });
        }

        internal virtual void UserUpdateStatus(MI_USER User)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = User.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pStatusID = new SqlParameter
            {
                ParameterName = "pStatusID",
                Value = User.STATUS_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIUserUpdateStatus @pUserID, @pStatusID",
                new object[] { pUserID, pStatusID});
        }
        internal virtual async Task UserUpdateStatusAsync(MI_USER User)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = User.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pStatusID = new SqlParameter
            {
                ParameterName = "pStatusID",
                Value = User.STATUS_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIUserUpdateStatus @pUserID, @pStatusID",
                new object[] { pUserID, pStatusID });
        }

        internal virtual bool UserValidatePassword(MI_USER User, string Password)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = User.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pPassword = new SqlParameter
            {
                ParameterName = "pPassword",
                Value = Password,
                SqlDbType = SqlDbType.NVarChar,
                Size = 30,
                Direction = ParameterDirection.Input
            };

            SqlParameter pResult = new SqlParameter
            {
                ParameterName = "pResult",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIUserValidatePassword @pUserID, @pPassword, @pResult OUTPUT",
                new object[] { pUserID, pPassword, pResult });

            return Convert.ToInt32(pResult.Value) == 1;
        }
        internal virtual async Task<bool> UserValidatePasswordAsync(MI_USER User, string Password)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = User.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pPassword = new SqlParameter
            {
                ParameterName = "pPassword",
                Value = Password,
                SqlDbType = SqlDbType.NVarChar,
                Size = 30,
                Direction = ParameterDirection.Input
            };

            SqlParameter pResult = new SqlParameter
            {
                ParameterName = "pResult",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIUserValidatePassword @pUserID, @pPassword, @pResult OUTPUT",
                new object[] { pUserID, pPassword, pResult });

            return Convert.ToInt32(pResult.Value) == 1;
        }

        //Warehouse Stored Procedures
        internal virtual int WarehouseInsert(MI_WAREHOUSE Warehouse)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = Warehouse.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pName = new SqlParameter
            {
                ParameterName = "pName",
                Value = Warehouse.NAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            //This is nullable and I can't find a better way to do this.
            SqlParameter pDescription;
            if (string.IsNullOrWhiteSpace(Warehouse.DESCRIPTION))
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = Warehouse.DESCRIPTION,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }

            SqlParameter pWarehouseID = new SqlParameter
            {
                ParameterName = "pWarehouseID",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIWarehouseInsert @pUserID, @pName, @pDescription, @pWarehouseID OUTPUT",
                new object[] { pUserID, pName, pDescription, pWarehouseID });

            //Get the generated ID and set it on the object and return it
            Warehouse.WAREHOUSE_ID = Convert.ToInt32(pWarehouseID.Value);
            return Convert.ToInt32(pWarehouseID.Value);
        }
        internal virtual async Task<int> WarehouseInsertAsync(MI_WAREHOUSE Warehouse)
        {
            //Set up the parameters
            SqlParameter pUserID = new SqlParameter
            {
                ParameterName = "pUserID",
                Value = Warehouse.USER_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pName = new SqlParameter
            {
                ParameterName = "pName",
                Value = Warehouse.NAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            //This is nullable and I can't find a better way to do this.
            SqlParameter pDescription;
            if (string.IsNullOrWhiteSpace(Warehouse.DESCRIPTION))
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            } else
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = Warehouse.DESCRIPTION,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }
            

            SqlParameter pWarehouseID = new SqlParameter
            {
                ParameterName = "pWarehouseID",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIWarehouseInsert @pUserID, @pName, @pDescription, @pWarehouseID OUTPUT",
                new object[] { pUserID, pName, pDescription, pWarehouseID });

            //Get the generated ID and set it on the object and return it
            Warehouse.WAREHOUSE_ID = Convert.ToInt32(pWarehouseID.Value);
            return Convert.ToInt32(pWarehouseID.Value);
        }

        internal virtual void WarehouseUpdateData(MI_WAREHOUSE Warehouse)
        {
            //Set up the parameters
            SqlParameter pWarehouseID = new SqlParameter
            {
                ParameterName = "pWarehouseID",
                Value = Warehouse.WAREHOUSE_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pName = new SqlParameter
            {
                ParameterName = "pName",
                Value = Warehouse.NAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            //This is nullable and I can't find a better way to do this.
            SqlParameter pDescription;
            if (string.IsNullOrWhiteSpace(Warehouse.DESCRIPTION))
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = Warehouse.DESCRIPTION,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIWarehouseUpdateData @pWarehouseID, @pName, @pDescription",
                new object[] { pWarehouseID, pName, pDescription });
        }
        internal virtual async Task WarehouseUpdateDataAsync(MI_WAREHOUSE Warehouse)
        {
            //Set up the parameters
            SqlParameter pWarehouseID = new SqlParameter
            {
                ParameterName = "pWarehouseID",
                Value = Warehouse.WAREHOUSE_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pName = new SqlParameter
            {
                ParameterName = "pName",
                Value = Warehouse.NAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            //This is nullable and I can't find a better way to do this.
            SqlParameter pDescription;
            if (string.IsNullOrWhiteSpace(Warehouse.DESCRIPTION))
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = Warehouse.DESCRIPTION,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIWarehouseUpdateData @pWarehouseID, @pName, @pDescription",
                new object[] { pWarehouseID, pName, pDescription });
        }

        internal virtual void WarehouseUpdateStatus(MI_WAREHOUSE Warehouse)
        {
            //Set up the parameters
            SqlParameter pWarehouseID = new SqlParameter
            {
                ParameterName = "pWarehouseID",
                Value = Warehouse.WAREHOUSE_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pStatusID = new SqlParameter
            {
                ParameterName = "pStatusID",
                Value = Warehouse.STATUS_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIWarehouseUpdateStatus @pWarehouseID, @pStatusID",
                new object[] { pWarehouseID, pStatusID });
        }
        internal virtual async Task WarehouseUpdateStatusAsync(MI_WAREHOUSE Warehouse)
        {
            //Set up the parameters
            SqlParameter pWarehouseID = new SqlParameter
            {
                ParameterName = "pWarehouseID",
                Value = Warehouse.WAREHOUSE_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pStatusID = new SqlParameter
            {
                ParameterName = "pStatusID",
                Value = Warehouse.STATUS_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIWarehouseUpdateStatus @pWarehouseID, @pStatusID",
                new object[] { pWarehouseID, pStatusID });
        }

        //Item Stored Procedures
        internal virtual int ItemInsert(MI_WH_ITEM Item)
        {
            //Set up the parameters
            SqlParameter pWarehouseID = new SqlParameter
            {
                ParameterName = "pWarehouseID",
                Value = Item.WAREHOUSE_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pName = new SqlParameter
            {
                ParameterName = "pName",
                Value = Item.NAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            //This is nullable and I can't find a better way to do this.
            SqlParameter pDescription;
            if (string.IsNullOrWhiteSpace(Item.DESCRIPTION))
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = Item.DESCRIPTION,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }

            //This is nullable and I can't find a better way to do this.
            SqlParameter pPrice;
            if (Item.PRICE == null)
            {
                pPrice = new SqlParameter
                {
                    ParameterName = "pPrice",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.Money,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pPrice = new SqlParameter
                {
                    ParameterName = "pPrice",
                    Value = Item.PRICE,
                    SqlDbType = SqlDbType.Money,
                    Direction = ParameterDirection.Input
                };
            }

            //This is nullable and I can't find a better way to do this.
            SqlParameter pUOM;
            if (string.IsNullOrWhiteSpace(Item.UOM))
            {
                pUOM = new SqlParameter
                {
                    ParameterName = "pUOM",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pUOM = new SqlParameter
                {
                    ParameterName = "pUOM",
                    Value = Item.UOM,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input
                };
            }

            SqlParameter pItemID = new SqlParameter
            {
                ParameterName = "pItemID",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIItemInsert @pWarehouseID, @pName, @pDescription, @pPrice, @pUOM, @pItemID OUTPUT",
                new object[] { pWarehouseID, pName, pDescription, pPrice, pUOM, pItemID });

            //Set the generated ID on the object and return it
            Item.ITEM_ID = Convert.ToInt32(pItemID.Value);
            return Convert.ToInt32(pItemID.Value);
        }
        internal virtual async Task<int> ItemInsertAsync(MI_WH_ITEM Item)
        {
            //Set up the parameters
            SqlParameter pWarehouseID = new SqlParameter
            {
                ParameterName = "pWarehouseID",
                Value = Item.WAREHOUSE_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pName = new SqlParameter
            {
                ParameterName = "pName",
                Value = Item.NAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            //This is nullable and I can't find a better way to do this.
            SqlParameter pDescription;
            if (string.IsNullOrWhiteSpace(Item.DESCRIPTION))
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = Item.DESCRIPTION,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }

            //This is nullable and I can't find a better way to do this.
            SqlParameter pPrice;
            if (Item.PRICE == null)
            {
                pPrice = new SqlParameter
                {
                    ParameterName = "pPrice",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.Money,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pPrice = new SqlParameter
                {
                    ParameterName = "pPrice",
                    Value = Item.PRICE,
                    SqlDbType = SqlDbType.Money,
                    Direction = ParameterDirection.Input
                };
            }

            //This is nullable and I can't find a better way to do this.
            SqlParameter pUOM;
            if (string.IsNullOrWhiteSpace(Item.UOM))
            {
                pUOM = new SqlParameter
                {
                    ParameterName = "pUOM",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pUOM = new SqlParameter
                {
                    ParameterName = "pUOM",
                    Value = Item.UOM,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input
                };
            }

            SqlParameter pItemID = new SqlParameter
            {
                ParameterName = "pItemID",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIItemInsert @pWarehouseID, @pName, @pDescription, @pPrice, @pUOM, @pItemID OUTPUT",
                new object[] { pWarehouseID, pName, pDescription, pPrice, pUOM, pItemID });

            //Set the generated ID on the object and return it
            Item.ITEM_ID = Convert.ToInt32(pItemID.Value);
            return Convert.ToInt32(pItemID.Value);
        }

        internal virtual void ItemUpdateData(MI_WH_ITEM Item)
        {
            //Set up the parameters
            SqlParameter pItemID = new SqlParameter
            {
                ParameterName = "pItemID",
                Value = Item.ITEM_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pName = new SqlParameter
            {
                ParameterName = "pName",
                Value = Item.NAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            //This is nullable and I can't find a better way to do this.
            SqlParameter pDescription;
            if (string.IsNullOrWhiteSpace(Item.DESCRIPTION))
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = Item.DESCRIPTION,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }

            //This is nullable and I can't find a better way to do this.
            SqlParameter pPrice;
            if (Item.PRICE == null)
            {
                pPrice = new SqlParameter
                {
                    ParameterName = "pPrice",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.Money,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pPrice = new SqlParameter
                {
                    ParameterName = "pPrice",
                    Value = Item.PRICE,
                    SqlDbType = SqlDbType.Money,
                    Direction = ParameterDirection.Input
                };
            }

            //This is nullable and I can't find a better way to do this.
            SqlParameter pUOM;
            if (string.IsNullOrWhiteSpace(Item.UOM))
            {
                pUOM = new SqlParameter
                {
                    ParameterName = "pUOM",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pUOM = new SqlParameter
                {
                    ParameterName = "pUOM",
                    Value = Item.UOM,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input
                };
            }

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIItemUpdateData @pItemID, @pName, @pDescription, @pPrice, @pUOM",
                new object[] { pItemID, pName, pDescription, pPrice, pUOM});
        }
        internal virtual async Task ItemUpdateDataAsync(MI_WH_ITEM Item)
        {
            //Set up the parameters
            SqlParameter pItemID = new SqlParameter
            {
                ParameterName = "pItemID",
                Value = Item.ITEM_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pName = new SqlParameter
            {
                ParameterName = "pName",
                Value = Item.NAME,
                SqlDbType = SqlDbType.NVarChar,
                Size = 100,
                Direction = ParameterDirection.Input
            };

            //This is nullable and I can't find a better way to do this.
            SqlParameter pDescription;
            if (string.IsNullOrWhiteSpace(Item.DESCRIPTION))
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pDescription = new SqlParameter
                {
                    ParameterName = "pDescription",
                    Value = Item.DESCRIPTION,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 250,
                    Direction = ParameterDirection.Input
                };
            }

            //This is nullable and I can't find a better way to do this.
            SqlParameter pPrice;
            if (Item.PRICE == null)
            {
                pPrice = new SqlParameter
                {
                    ParameterName = "pPrice",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.Money,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pPrice = new SqlParameter
                {
                    ParameterName = "pPrice",
                    Value = Item.PRICE,
                    SqlDbType = SqlDbType.Money,
                    Direction = ParameterDirection.Input
                };
            }

            //This is nullable and I can't find a better way to do this.
            SqlParameter pUOM;
            if (string.IsNullOrWhiteSpace(Item.UOM))
            {
                pUOM = new SqlParameter
                {
                    ParameterName = "pUOM",
                    Value = DBNull.Value,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input
                };
            }
            else
            {
                pUOM = new SqlParameter
                {
                    ParameterName = "pUOM",
                    Value = Item.UOM,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input
                };
            }

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIItemUpdateData @pItemID, @pName, @pDescription, @pPrice, @pUOM",
                new object[] { pItemID, pName, pDescription, pPrice, pUOM });
        }

        internal virtual void ItemUpdateStatus(MI_WH_ITEM Item)
        {
            //Set up the parameters
            SqlParameter pItemID = new SqlParameter
            {
                ParameterName = "pItemID",
                Value = Item.ITEM_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pStatusID = new SqlParameter
            {
                ParameterName = "pStatusID",
                Value = Item.STATUS_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIItemUpdateStatus @pItemID, @pStatusID",
                new object[] { pItemID, pStatusID });
        }
        internal virtual async Task ItemUpdateStatusAsync(MI_WH_ITEM Item)
        {
            //Set up the parameters
            SqlParameter pItemID = new SqlParameter
            {
                ParameterName = "pItemID",
                Value = Item.ITEM_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pStatusID = new SqlParameter
            {
                ParameterName = "pStatusID",
                Value = Item.STATUS_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIItemUpdateStatus @pItemID, @pStatusID",
                new object[] { pItemID, pStatusID });
        }

        internal virtual int ItemTransaction(MI_WH_ITEM Item, MI_TRANSACTIONTYPE_REF Transaction, int Amount)
        {
            //Set up the parameters
            SqlParameter pItemID = new SqlParameter
            {
                ParameterName = "pItemID",
                Value = Item.ITEM_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pTransactionTypeID = new SqlParameter
            {
                ParameterName = "pTransactionTypeID",
                Value = Transaction.TRANSACTIONTYPE_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pAmount = new SqlParameter
            {
                ParameterName = "pAmount",
                Value = Amount,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pNewAmount = new SqlParameter
            {
                ParameterName = "pNewAmount",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            Database.ExecuteSqlCommand("EXEC MIItemTransaction @pItemID, @pTransactionTypeID, @pAmount, @pNewAmount OUTPUT",
                new object[] { pItemID, pTransactionTypeID, pAmount, pNewAmount });

            //Set the amount on the item and return it
            Item.QUANTITY = Convert.ToInt32(pNewAmount.Value);
            return Convert.ToInt32(pNewAmount.Value);
        }
        internal virtual async Task<int> ItemTransactionAsync(MI_WH_ITEM Item, MI_TRANSACTIONTYPE_REF Transaction, int Amount)
        {
            //Set up the parameters
            SqlParameter pItemID = new SqlParameter
            {
                ParameterName = "pItemID",
                Value = Item.ITEM_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pTransactionTypeID = new SqlParameter
            {
                ParameterName = "pTransactionTypeID",
                Value = Transaction.TRANSACTIONTYPE_ID,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pAmount = new SqlParameter
            {
                ParameterName = "pAmount",
                Value = Amount,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            SqlParameter pNewAmount = new SqlParameter
            {
                ParameterName = "pNewAmount",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            //Execute the stored procedure
            await Database.ExecuteSqlCommandAsync("EXEC MIItemTransaction @pItemID, @pTransactionTypeID, @pAmount, @pNewAmount OUTPUT",
                new object[] { pItemID, pTransactionTypeID, pAmount, pNewAmount });

            //Set the amount on the item and return it
            Item.QUANTITY = Convert.ToInt32(pNewAmount.Value);
            return Convert.ToInt32(pNewAmount.Value);
        }
    }
}
