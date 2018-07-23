namespace Nova.SearchAlgorithm.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<SearchAlgorithmContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Nova.SearchAlgorithm.Data.SearchAlgorithmContext";
            CommandTimeout = 0;
        }

        protected override void Seed(SearchAlgorithmContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
