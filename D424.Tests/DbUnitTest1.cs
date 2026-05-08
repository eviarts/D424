using System;
using System.IO;
using Xunit;
using D424.Classes;

namespace D424.Tests;

public class DbUnitTest1
{
    private Database DB()
    {
        var path = Path.GetTempFileName();
        return new Database(path);
    }

    [Fact]
    public async Task SaveInsert()
    {
        var db = DB();
        await db.TestInitAsync();

        var term = new Terms
        {
            TermTitle = "Term 1",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddMonths(1)
        };
        await db.SaveTermAsync(term);

        var result = await db.GetTermsAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task DeleteTerm()
    {
        var db = DB();
        await db.TestInitAsync();

        //Insert term to delete
        var term = new Terms
        {
            TermTitle = "Test Term",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddMonths(1)
        };
        await db.SaveTermAsync(term);

        var toDelete = await db.GetTermsAsync();

        Assert.Single(toDelete);
        
        //Delete inserted
        await db.DeleteTermAsync(term);

        var deleted = await db.GetTermsAsync();

        Assert.Empty(deleted);
                    
    }
}
