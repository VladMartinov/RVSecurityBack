namespace Tests.Integration.TestContainers.Pg;

[CollectionDefinition("Postgres collection")]
public class PostgresCollection : ICollectionFixture<PostgresContainerFixture>
{
}
