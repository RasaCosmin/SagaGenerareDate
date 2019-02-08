namespace ValidareDate.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ValidareDate.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ValidareDate.Data.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ValidareDate.Data.AppDbContext";
        }

        protected override void Seed(ValidareDate.Data.AppDbContext context)
        {
            var judete = new List<Judet>(){
                 new Judet{ID="01", Name="Alba", Cod="AB"},
                 new Judet{ID="02", Name="Arad", Cod="AR"},
                 new Judet{ID="03", Name="Argeș", Cod="AG" },
                 new Judet{ID="04", Name="Bacău", Cod="BC"},
                 new Judet{ID="05", Name="Bihor", Cod="BH"},
                 new Judet{ID="06", Name="Bistrița-Năsăud", Cod="BN"},
                 new Judet{ID="07", Name="Botoșani", Cod="BT"},
                 new Judet{ID="08", Name="Brașov", Cod="BVV"},
                 new Judet{ID="09", Name="Brăila", Cod="BR"},
                 new Judet{ID="10", Name="Buzău", Cod="BZ"},
                 new Judet{ID="11", Name="Caraș-Severin", Cod="CS"},
                 new Judet{ID="12", Name="Cluj", Cod="CJ"},
                 new Judet{ID="13", Name="Constanța", Cod="CT"},
                 new Judet{ID="14", Name="Covasna", Cod="CV"},
                 new Judet{ID="15", Name="Dâmbovița", Cod="DB"},
                 new Judet{ID="16", Name="Dolj", Cod="BJ"},
                 new Judet{ID="17", Name="Galați", Cod="GL"},
                 new Judet{ID="18", Name="Gorj", Cod="GJ"},
                 new Judet{ID="19", Name="Harghita", Cod="HR"},
                 new Judet{ID="20", Name="Hunedoara", Cod="HD"},
                 new Judet{ID="21", Name="Ialomița", Cod="IL"},
                 new Judet{ID="22", Name="Iași", Cod="IS"},
                 new Judet{ID="23", Name="Ilfov", Cod="IF"},
                 new Judet{ID="24", Name="Maramureș", Cod="MM"},
                 new Judet{ID="25", Name="Mehedinți", Cod="MH"},
                 new Judet{ID="26", Name="Mureș", Cod="MS"},
                 new Judet{ID="27", Name="Neamț", Cod="NT"},
                 new Judet{ID="28", Name="Olt", Cod="OT"},
                 new Judet{ID="29", Name="Prahova", Cod="PH"},
                 new Judet{ID="30", Name="Satu Mare", Cod="SM"},
                 new Judet{ID="31", Name="Sălaj", Cod="SJ"},
                 new Judet{ID="32", Name="Sibiu", Cod="SB"},
                 new Judet{ID="33", Name="Suceava", Cod="SV"},
                 new Judet{ID="34", Name="Teleorman", Cod="TR"},
                 new Judet{ID="35", Name="Timiș", Cod="TM"},
                 new Judet{ID="36", Name="Tulcea", Cod="TL"},
                 new Judet{ID="37", Name="Vaslui", Cod="VS"},
                 new Judet{ID="38", Name="Vâlcea", Cod="VL"},
                 new Judet{ID="39", Name="Vrancea", Cod="VN"},
                 new Judet{ID="40", Name="București", Cod="B"},
                 new Judet{ID="41", Name="București - Sector 1", Cod="B"},
                 new Judet{ID="42", Name="București - Sector 2", Cod="B"},
                 new Judet{ID="43", Name="București - Sector 3", Cod="B"},
                 new Judet{ID="44", Name="București - Sector 4", Cod="B"},
                 new Judet{ID="45", Name="București - Sector 5", Cod="B"},
                 new Judet{ID="46", Name="București - Sector 6", Cod="B"},
                 new Judet{ID="47", Name="București - Sector 7", Cod="B"},
                 new Judet{ID="48", Name="București - Sector 8", Cod="B"},
                 new Judet{ID="51", Name="Călărași", Cod="CL"},
                 new Judet{ID="52", Name="Giurgiu", Cod="GR"}

            };

            context.Judete.AddOrUpdate(judete.ToArray());
        }
    }
}
