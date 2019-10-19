# LiquidVictor

**A media tracking and aggregation system that can be used to build presentations.**

The goal of *LiquidVictor* is to make building presentations easier.  Many presentations need to 
evolve over time but often use assets from previous versions or even other presentations. 
If presentation assets can be organized and tracked so that the basic structure of presentations 
can be put together by selecting and ordering these existing assets, it would make creating 
and maintaining these presentations much easer.

* Assets that could be tracked include:
  * Slide content
  * Images
  * Charts and graphs
  * Links and their thumbnails
  * Code samples

* Some key features of the system may be:
  * Generate presentations in multiple formats and with different themes and aspect ratios
  * Allow the regeneration of all presentations containing a slide or image when that asset changes
  * Support the labeling of states so that a presentation can be regenerated as it was at a particular point in time


Prototypes and contribution guidelines are forthcoming.

To build the project, take the following steps:

1) Download the code repository and open the LiquidVictor solution file in Visual Studio or equivalent IDE.
1) From a console window at the root of the code repository, execute the git command `git update-index --skip-worktree \src\lv\Properties\launchSettings.json` to tell git not to upload any changes to that file since they are specific to your local installation.
1) Modify the `\src\lv\Properties\launchSettings.json` file to point to the presentation you wish to build (sample presentation repositories are forthcoming).
1) Execute the LV.exe CLI by pressing F5 in Visual Studio or invoking the proper command in your IDE.
