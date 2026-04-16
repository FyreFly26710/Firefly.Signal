using Firefly.Signal.JobSearch.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Firefly.Signal.JobSearch.UnitTests.Domain;

[TestClass]
public sealed class JobApplicationTests
{
    [TestMethod]
    public void Create_WhenNoteHasPadding_TrimsStoredValue()
    {
        var application = JobApplication.Create(userAccountId: 42, jobPostingId: 99, note: "  First contact made  ");

        Assert.AreEqual("First contact made", application.Note);
    }

    [TestMethod]
    public void UpdateNote_WhenNoteIsWhitespace_NormalizesToNull()
    {
        var application = JobApplication.Create(userAccountId: 42, jobPostingId: 99, note: "Initial note");

        application.UpdateNote("   ");

        Assert.IsNull(application.Note);
    }
}
