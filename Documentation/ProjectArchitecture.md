# Project Architecture
_Design & Intentions_

###Goals###

_The following are my personal goals for DinkPDN. These do not reflect any shared desires of the rest of the PDN community_

  1. Plugins should each compile to a single-assembly and be limited to only referencing core PDN/system libraries.
  2. Plugins should maximize usage of common codebases. Code in plugin assembly projects should be minimized to the needs specific only to the function of the plugin where possible.
  3. Plugins should attempt to utilize existing UI as much as possible in order to provide a consistent user experience for the community.

###Solution###

I have decided to go with a single solution to house all of the plugins that I am making.
While there are other ways to accomplish sharing code between "single-compile" assemblies I like the format of Shared Projects.
Shared Projects will be heavily utilized to build a common codebase / framework for my plugins.

###Shared Projects###

Shared Projects will hold only the shared, common code for DinkPDN based plugins. 
These should favor a "many & small" over "few & large" design approach to limit plugin bloat.
In order to avoid painful refactors down the road there is a desire to put considerable effort in the naming / namespacing of these projects.

###Shared Project Unit Tests###

Unit test projects are unable to make reference to Shared Projects and so it is acceptable to create test assemblies in order to run tests on.

###Plugin Projects###

Then general rule of thumb here is that one project should ultimately provide a single menu item in PDN.
These projects should only reference core PDN libraries and DinkPDN Shared Projects.
References should not be made to Shared Projects that will not be utilized by the plugin.
This will help to ensure minimal bloat to the individual plugin assembly when compiled.

###User Experience###

All plugins should attempt to utilize existing UI components in order to provide a consistent and familiar experience for users.
I will even allow a little bit of cheating at the expense of possible risk to compatability to keep a consistent user experience.
While consistency is of high importance, plugin UI should strive to be self-explanatory. 
A good goal to have is that a user doesn't need to seek out documentation / examples to use the plugin.
I am not against incorporating complicated configurations, but would prefer that my plugins are less "Kitchen Sink" and more "Single Responsibility".
I, personally, prefer plugins that do one job well and concisely over plugins that try to do everything at the expense of complicating the primary function.


