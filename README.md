# PV260 Software Quality project


## Process Modeling & Event Storming
We have performed a thorough process modeling and event storming to better visualize and understand the system's flow.

You can view the detailed process models and event storming diagrams in [Figma](https://www.figma.com/design/2nc9rLTN6H6vEw8kRaJcw4/Event-Storming-(Community)?node-id=48-13171&t=HooV3KFXS7eQsvSa-1).

## User Stories
The user stories have been meticulously created and organized in [Jira](https://pv260.atlassian.net/jira/software/projects/IN/boards/1) for ease of tracking and development.
To access the Jira you need to log in with **an account from @mail.muni.cz** domain.

Each of us voted on story points to estimate the effort for each user story. The majority of us voted similarly on each of the tasks.

## Git Conventions
When working on a story, add its key to the branch name and commit message so that it can be linked with Jira. Also include the Jira key in the pull request name.

## Generating Solution Files
`.sln` files are excluded from git and need to be generated locally. To do so, run the following command in the `src` directory:
```bash
slngen dirs.proj -o PV260.sln --launch false
```
> **NOTE:** The `--launch false` parameter prevents Visual Studio from opening the solution file automatically. You can omit it if you want to open the solution file in Visual Studio right after generating it.

## CI/CD Pipeline
GitHub Actions workflows are used for CI/CD. For each Pull Request, a unit tests check is run. Also, after each commit to main, client and server binaries are compiled and a (pre)release is autogenerated.
Furthermore, a Docker image is built for the API server, pushed to the Packages repository, and deployed on the Staging server.

After the release is tested on the Staging server, it can be promoted to Production by creating a new release with a version tag in the format `vX.Y.Z`,
where X is the major version, Y is the minor version, and Z is the patch version.
