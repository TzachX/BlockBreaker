# BlockBreaker
Communix unity developer home assignment

Hi! I hope you'll be satisfied from the project and my code.
By Shahar's advice, I wrapped the code in a single ASMDF/Namespace, since the original code is not well written, and has cyclic dependencies which caused errors.

Implementation of S.O.L.I.D in ConfigurationManager:
S - This class is responsible *only* for getting the configuration, nothing else.
O - This class can expand and have more functions, but nothing is needed to be changed/removed in order for new changes to work.
L - In GetConfig we don't need to know what BaseConfig is to work with it. When invoking the event BBConfig is sent, and for other configs we can use more events in the future.
I - No interfaces are used, but anyways there are no forced dependencies in this class.
D - BBConfig is based on BaseConfig, which is an abstract class, since we don't need an instance of BaseConfig in the project.

P.S: Assets/Firebase/Plugins/x86_64/FirebaseCppApp-11_5_0.bundle was larger than 100MB so I had to remove it from the commit, I hope it's not an issue
