# Identify

Identify is a simple, open-source .Net 4.5 .dll for retrieving unique identifying information from common file system objects, such as files and folders

## Can't This Be Done In C++

Yes. Absolutely. However, sometimes it's just easier - and maybe more fun - to have this "stuff" written in C#.

## Okay. I Believe You. Tell Me The Deets

Much of the code for this solution is derived from Microsoft's [Local File System](https://msdn.microsoft.com/en-us/library/windows/desktop/aa364407(v=vs.85).aspx) documentation, specifically [Directory Management](https://msdn.microsoft.com/en-us/library/windows/desktop/bb540529(v=vs.85).aspx) and [File Management](https://msdn.microsoft.com/en-us/library/windows/desktop/bb540531(v=vs.85).aspx). Specifically, the solution takes advantage of the handles received as a result of the cheap `FILE_CREATE` function exposed by Microsoft. The process for retrieving a handle for a file on the file system is relatively straigtforward; however, for a directory it requires a bit more digging (nothing too advanced). 

## The Code Is Simple. Why a .dll? 

This is meant to make inclusion in your project a bit simpler; however, you have access to the code. If grabbing the source is easier - or, if you want to use the source to expand upon - do it. 

## Your Solution Sucks

That may really be the case. If you have the time to make it better, please feel free to contribute changes. As I mentioned, above, you have access to the source. 

## Windows Support 


## Known Limitations
