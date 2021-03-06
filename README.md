# BinaryToolkit

## Why BinaryToolkit is nice?

- It works on any x64 Windows OS
- It works on Windows XP
- It works on [ReactOS](https://github.com/reactos/reactos)
- It ~~works~~ will work on Linux, Mac, Android on any device because of .NET Core port (branch will be added later)

## Usage

```csharp
//
// Create BinaryAccess instance
//
// With this argument, it will firstly look for "speed" process,
// Then for "speed" file
//
BinaryAccess mem = new BinaryAccess("speed");

// Then do everything you want.


// Read from Main Module from address 0x8B26D4
string text = mem.Read<string>((IntPtr)0x8B26D4);

// Methods in BinaryAccess are wrappers for MainModule methods.
// So the same will be in this case:
text = mem.MainModule.Read<string((IntPtr)0x8B26D4);

// You have an access to modules memory (only for processes!)
foreach (var module in mem.Modules)
{
	WriteLine("[Module] " + module.ToString());
	string result = module.Read<string((IntPtr)0x8B26D4);
}

// Don't forget to Dispose BinaryAccess to free all resources.
mem.Dispose();
```
