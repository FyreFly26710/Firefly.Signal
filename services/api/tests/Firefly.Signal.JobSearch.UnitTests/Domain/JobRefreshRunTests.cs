using Firefly.Signal.JobSearch.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.UnitTests.Domain;

[TestClass]
public sealed class JobRefreshRunTests
{
    [TestMethod]
    public void Start_WhenArgumentsHavePadding_NormalizesStoredValues()
    {
        var run = JobRefreshRun.Start(
            providerName: "  Adzuna  ",
            countryCode: "  GB  ",
            requestFiltersJson: "  {\"where\":\"london\"}  ",
            requestedPageSize: 50,
            requestedMaxPages: 1);

        Assert.AreEqual("Adzuna", run.ProviderName);
        Assert.AreEqual("gb", run.CountryCode);
        Assert.AreEqual("{\"where\":\"london\"}", run.RequestFiltersJson);
        Assert.AreEqual(JobRefreshRunStatus.Running, run.Status);
        Assert.IsFalse(run.IsTerminal);
    }

    [TestMethod]
    public void RecordHiddenJobs_WhenHiddenWouldExceedInserted_Throws()
    {
        var run = CreateRun();
        run.RecordInsertedJobs(2);

        AssertThrows<InvalidOperationException>(() => run.RecordHiddenJobs(3));
    }

    [TestMethod]
    public void Fail_WhenMessageHasPadding_TrimsAndMarksRunTerminal()
    {
        var run = CreateRun();

        run.RecordFailedItems(1);
        run.Fail("  Provider import failed.  ");

        Assert.AreEqual(JobRefreshRunStatus.Failed, run.Status);
        Assert.AreEqual("Provider import failed.", run.FailureMessage);
        Assert.AreEqual("Provider import failed.", run.FailureSummary);
        Assert.IsTrue(run.HasFailures);
        Assert.IsTrue(run.IsTerminal);
        Assert.IsNotNull(run.CompletedAtUtc);
    }

    [TestMethod]
    public void Complete_WhenJobsWereInsertedAndHidden_ComputesVisibleInsertedCount()
    {
        var run = CreateRun();

        run.RecordFetchedPage(5);
        run.RecordInsertedJobs(4);
        run.RecordHiddenJobs(1);
        run.Complete();

        Assert.AreEqual(3, run.VisibleRecordsInserted);
        Assert.AreEqual(JobRefreshRunStatus.Completed, run.Status);
        Assert.IsTrue(run.IsTerminal);
    }

    [TestMethod]
    public void RecordFetchedPage_AfterCompletion_Throws()
    {
        var run = CreateRun();
        run.Complete();

        AssertThrows<InvalidOperationException>(() => run.RecordFetchedPage(1));
    }

    private static JobRefreshRun CreateRun()
        => JobRefreshRun.Start(
            providerName: "Adzuna",
            countryCode: "gb",
            requestFiltersJson: "{\"where\":\"london\"}",
            requestedPageSize: 50,
            requestedMaxPages: 1);

    private static void AssertThrows<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
            Assert.Fail($"Expected {typeof(TException).Name} to be thrown.");
        }
        catch (TException)
        {
        }
    }
}
