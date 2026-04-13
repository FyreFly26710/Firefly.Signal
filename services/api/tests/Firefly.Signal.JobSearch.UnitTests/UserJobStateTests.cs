using Firefly.Signal.JobSearch.Domain;

namespace Firefly.Signal.JobSearch.UnitTests;

[TestClass]
public class UserJobStateTests
{
    [TestMethod]
    public void Create_initialises_with_both_flags_false()
    {
        var state = UserJobState.Create(1, 100);

        Assert.IsFalse(state.IsSaved);
        Assert.IsFalse(state.IsHidden);
    }

    [TestMethod]
    public void MarkSaved_sets_IsSaved_true()
    {
        var state = UserJobState.Create(1, 100);

        state.MarkSaved();

        Assert.IsTrue(state.IsSaved);
    }

    [TestMethod]
    public void Unsave_clears_IsSaved()
    {
        var state = UserJobState.Create(1, 100);
        state.MarkSaved();

        state.Unsave();

        Assert.IsFalse(state.IsSaved);
    }

    [TestMethod]
    public void Hide_sets_IsHidden_true()
    {
        var state = UserJobState.Create(1, 100);

        state.Hide();

        Assert.IsTrue(state.IsHidden);
    }

    [TestMethod]
    public void Unhide_clears_IsHidden()
    {
        var state = UserJobState.Create(1, 100);
        state.Hide();

        state.Unhide();

        Assert.IsFalse(state.IsHidden);
    }

    [TestMethod]
    public void IsSaved_and_IsHidden_are_independent()
    {
        var state = UserJobState.Create(1, 100);

        state.MarkSaved();
        state.Hide();
        Assert.IsTrue(state.IsSaved);
        Assert.IsTrue(state.IsHidden);

        state.Unsave();
        Assert.IsFalse(state.IsSaved);
        Assert.IsTrue(state.IsHidden);

        state.Unhide();
        Assert.IsFalse(state.IsSaved);
        Assert.IsFalse(state.IsHidden);
    }
}
