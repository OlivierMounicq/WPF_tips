For your C# application with two Docker containers accessing the same MongoDB database, here’s how to prevent dirty reads:
Use Read Concern “Majority”
In your C# MongoDB driver, configure read concern at the collection or operation level:

var readConcern = ReadConcern.Majority;
var collection = database.GetCollection<YourDocument>("collectionName")
    .WithReadConcern(readConcern);

// Now all reads from this collection use majority read concern
var result = await collection.Find(filter).ToListAsync();


Or set it per operation:

var options = new FindOptions { ReadConcern = ReadConcern.Majority };
var result = await collection.Find(filter, options).ToListAsync();


Use Transactions for Critical Operations
When you need to read and write together atomically, use transactions with snapshot isolation:

using (var session = await client.StartSessionAsync())
{
    session.StartTransaction(new TransactionOptions(
        readConcern: ReadConcern.Snapshot,
        writeConcern: WriteConcern.WMajority
    ));

    try
    {
        var collection = database.GetCollection<YourDocument>("collectionName");
        
        // Read - guaranteed consistent snapshot
        var doc = await collection.Find(session, filter).FirstOrDefaultAsync();
        
        // Modify
        doc.Value += 1;
        
        // Write
        await collection.ReplaceOneAsync(session, filter, doc);
        
        await session.CommitTransactionAsync();
    }
    catch
    {
        await session.AbortTransactionAsync();
        throw;
    }
}


Configure Write Concern Too
Pair your read concern with an appropriate write concern to ensure consistency:

var writeConcern = WriteConcern.WMajority; // Wait for majority acknowledgment
var collection = database.GetCollection<YourDocument>("collectionName")
    .WithReadConcern(ReadConcern.Majority)
    .WithWriteConcern(writeConcern);


Important Prerequisites
Make sure your MongoDB deployment is a replica set (not standalone), even if it’s just a single-node replica set. Read concern “majority” requires this. If you’re using standalone MongoDB, you’ll need to convert it to a replica set first.
The combination of read concern “majority” and write concern “w: majority” ensures that both containers see consistent, committed data and prevents dirty reads entirely.​​​​​​​​​​​​​​​​