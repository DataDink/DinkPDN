# Project Architecture
_Design & Intentions_

##Goals##
_The following are my personal goals for DinkPDN. These do not reflect any shared desires of the rest of the PDN community_
  1. Plugins should each compile to a single-assembly and be limited to only referencing core PDN/system libraries.
  2. Plugins should maximize usage of common codebases. Code in plugin assembly projects should be minimized to the needs specific only to the function of the plugin where possible.
  3. Plugins should attempt to utilize existing UI as much as possible in order to provide a consistent user experience for the community.

##Solution##
I have decided to go with a single solution to house all of the plugins that I am making.
While there are other ways to accomplish sharing code between "single-compile" assemblies I like the format of Shared Projects.
Shared Projects will be heavily utilized to build a common codebase / framework for my plugins.

##Shared Projects##
Shared Projects will hold only the shared, common code for DinkPDN based plugins. 
These should favor a "many & small" over "few & large" design approach to limit plugin bloat.
In order to avoid painful refactors down the road there is a desire to put considerable effort in the naming / namespacing of these projects.

##Shared Project Unit Tests##
Unit test projects are unable to make reference to Shared Projects and so it is acceptable to create test assemblies in order to run tests on.

##Plugin Projects##
Then general rule of thumb here is that one project should ultimately provide a single menu item in PDN.
I am not against incorporating complicated configurations, but would prefer that my plugins are less "Kitchen Sink" and more "Single Responsibility".
I, personally, prefer plugins that do one job well and easily over plugins that try to do everything at the expense of complicating the primary function.


