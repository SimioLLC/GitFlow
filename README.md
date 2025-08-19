# GitFlow
If you are new to Git and source control, an introduction can be found at the root of this directory titled, "Simio with Version Control Software Overview".

This library exposes core Git functions in Simio. It relies on PAT authentication for repositories. The document can guide you with that for GitHub. There are many other Git repository systems. You may need to contact your I.T. administrator for first time repository setup. It is expected that you will come with a remote repository to leverage GitFlow.

There are several reasons to learn to work with Git while developing your Simio model. A few highlights:

1.	Commit log. Every time you are confident in a model change, you document all the differences from the last save, then you commit, and finally push the change to be part of the repository’s permanent history. If working in a team environment, this is informative.
2.	Have a peer review a large change. GitFlow, on saving the model locally, will prompt you to move your work into a DEV branch. That branch can be pushed as well. A peer can pull down the repository, switch from main to your dev branch, and review all model changes for themselves.
3.	Undo a large change. Using the flow described in (2) above, you can simply delete your dev branch and revert to main.
4.	Cleaner file management for a team. Everything described above today is files. Simio project files are often named in various ways to maintain edits and describe latest progress. If you have supporting data files, a python script, etc. that coincides with your model, it can all be in the git repository. To share, your peer simply clones the repository and when there is a change performs a git pull all via Simio with this add-in. All changes are applied.