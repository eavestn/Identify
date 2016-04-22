# Identify

Identify is a simple, open-source .Net 4.5 .dll for retrieving unique identifying information from common file system objects, such as files and folders

### Can't This Be Done In C++

Yes. Absolutely. However, sometimes it's just easier - and maybe more fun - to have this "stuff" written in C#. Really, one of the main motivations behind this project was to have this written in C# (see below for second motivation).

### Tell Me The Deets

Much of the code for this solution is derived from Microsoft's [Local File System](https://msdn.microsoft.com/en-us/library/windows/desktop/aa364407(v=vs.85).aspx) documentation, specifically [Directory Management](https://msdn.microsoft.com/en-us/library/windows/desktop/bb540529(v=vs.85).aspx) and [File Management](https://msdn.microsoft.com/en-us/library/windows/desktop/bb540531(v=vs.85).aspx). The solution takes advantage of the handles received as a result of the cheap `FILE_CREATE` function exposed by Microsoft. The process for retrieving a handle for a file on the file system is relatively straigtforward; however, for a directory it requires a bit more documentation digging (nothing too advanced). 

There are a number of different approaches to grabbing unique file-identifying information from the file system on the interwebs;  however; there is less information how to implement a solution to do so for directories, themselves. The _solution for grabbing unique directory-identifying information is more unique to this project_ and I wanted to share the approach in hopes of improving it.

### Why a .dll? 

This is meant to make inclusion in your project a bit simpler; however, you have access to the code. If grabbing the source is easier - or, if you want to use the source to expand upon - do it (and it probably makes more sense to do so). Eventually, I hope to expand this project to provide a more general, C#-based library for interacting with the file system. 

### Always Room For Improvement

That may really be the case. You may have a better approach than I do. If you have the time to make it better, please feel free to contribute changes. As I mentioned, above, you have access to the source. 

### Windows Support 

Minimum Supported Client | Minimum Supported Server
--- | --- 
Windows XP (desktop app.s only) | Windows Server 2003 (desktop app.s only)
