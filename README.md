[If you liked it, give me on paypal ! ;)](https://www.paypal.me/CorentinZ/1)

# ADOSS
## (Authentication and Document Oriented Storage System)

### Introduction
Are you fed up with creating again and again an authentication system in each of your project ? So am I !
That's why I created this wonderful system which allows you to authenticate all your users between all your services with this only server. Furthermore, the user can store his ciphered datas (texts, photos, videos, all that he needs), without recreating a new storage system as well. This way, you can develop a multi platform application, and sharing all datas between them.
And because the entire system is develop in C# .NET Core, you can install it on Windows, Linux, and Mac.
Also, you can implement your own language for the system without recompliling the project or working on the code, it's made for.
So forget the hard work, and choose the simplicity.


### Usage
There is nothing easier than using this system. Once the server is launched, it is autonomous. Just connect to it and dialog with simple messages. You can register to a given service, sign in, get set or delete an object in the storage, and so on.
Server administrator can easily manage and interact with the system. Even if the system is listening to clients, he is also listening to local commands, to create users (same as registering on the client side), to create services, accept registers, delete users, and it's not exhaustive. (All modules and commands are explained after)

### Installation
There is nothing more simple than the installation of the server. Just. Launch. It.
Client side, just develop the api dialog depending on the language you use, and that's it ! this way you will retrieve all what you need.

File structure tree :
<pre><code>
-- datas/
   |-- store/
   |   |-- [access name]/
   |       |-- [user name]/
   |           |-- [key]        -- Containing the value
   |-- accesses              -- Containing all ciphered accesses
   |-- users                 -- Containing all ciphered users
-- lang/
   |-- en                    --Containing the english language
   |-- [lang]                --The language you want (same appearing in the configuration file)
-- logs/
   |-- localcmd.log          --Containing all success administration commands.
   |-- socketcmd.log         --Containing all success online commands.
-- config                   --Containing all server configuration.
</code></pre>

### Modules
##### Server configuration

The server is made to work with a configuration file, loaded at launch. Don't wory if you don't have it at the first launch, it will be created with default values. As seen in the file structure tree, the configuration file is at the server base directory.
Here are the configuration possibilities :

|Key|Default value|Explanation|
|---|:-----------:|-----------|
|lang|en|Server and error codes language|
|port|32000|Server listening port|
|maxconn|10|Max simultaneous connection handling|
|maxstr|50|Max character length displayed per message in the administration console|
|loglength|1000|Line number contained in log files before creating a new file|
|titlecolor1|Red|Primary color for the server title in the administration console (COSMETIC)|
|titlecolor2|Green|Secondary color for the server title in the administration console (COSMETIC)|

You can comment lines in the file as you want, with the **#** as first character of the line.
If a key as well as a value is misunderstood by the configuration reader, it will be discarded.
If a key is defined mutliple times, only the last occurence will be considered.
<br/>
##### Language system
The entire system is written in english by default. Because not everyone and not every system are in English, it's possible to adapt the system to run in another language. To translate strings, just write a new file based on the "en" file, name it with the name you want. The name needs to match the name in the configuration file. If the configuration loader is not able to read your language file, the "en" file will be used.
<br/>
##### Handling client and network protocol
Server / Client dialog is very simple. But first, let's have a look to the client handling by the server.
When the server is launched, it prepares the configuration and all modules it needs. Then, it turns into a socket listener, which wait for a client connection. As soon as a client connects to the server, the server creates a new thread in order to manage the client, and the server directly waits for another client.
Then, let's look at the thread method. In fact, the thread has one goal : Wait for a request, process the request, and answer. Then close the connection.

Here is the exact thread process :
> 1. Client connects.
> 1. Server send him its public key. (Cryptography system explained after)
> 1. Server waits for the client request.
> 1. Server processes the request and creates its answer.
> 1. Server sends the answer.
> 1. Server closes the connection.

_All requests need to be 64 bits string in order to be processed. That's the way the client can send files, music, or videos, and turn the server into a document oriented store system._
<br/>
##### Cryptography System
The cryptography system is a little bit more tricky. This system is used at three different places.

- **First**, the easier one, is for the store system. Both accesses and users are store ciphered in the data files. They are stored using the **Tornado algorithm**.
The Tornado algorithm is a simple symetric crypting algorithm.
Here are its steps :
    > 1. It generates a 6 digits long number by taking number of miliseconds of UNIX timestamp and extracting its 6 last digists.
    > 1. It creates a second number by reversing the first number.
    > 1. Then, it transforms by substitution the string with the first number.
    > 1. Same with the second number.
    > 1. And same with the first number again.
    > 1. It concatenates the number with the result string.
    > 1. It transforms the result with the same process and these numbers : 619743, 164792, 986521.

    _The reverse process is done for decrypting the string._

- **Secondly**, the cryptography system is used for the client / server dialogs.
Here, two cryptographic methods are used : The RijndaelManaged method, and RSA.
Here are the steps for encryption :
    > 1. Server generates the Key and IV for RijndaelManaged alorithm.
    > 1. Server encrypt its publickey and the message with the Key and IV.
    > 1. Server encrypt the Key and IV with the client public key (because symetric is faster than asymetric)
    > 1. Server send the message (ciphered message + ciphered Key + ciphered IV)

    Here are the steps for decryption :
    > 1. Recieve the message and extract all ciphered informations
    > 1. Decrpyt Key and IV with own private key.
    > 1. Decrpyt the message with the Key and IV.

- **The last** use case of cryptographic system is for the login token generation, described in the following part !
<br/>
##### Authentication and Storage system
The authentication system is used by the client to access a given service, by giving the server a username and a password, and of course, the target access.
Client has multiple choice :
1. **Connect to a service.**
    _User must exist.
    User must be active.
    Access must exist.
    User must have the access to the service.
    User give the right password for this service._
    <br/>
1. **Register to a service**
    _User must exist.
    User must be active.
    Access must exist.
    User needs not to have the access already._
    <br/>
1. **Change his password**
    _User must exist.
    User must be active.
    Access must exist.
    User must have access to the service.
    Old password needs to match.
    New password needs to be different._
    <br/>

With a granted access, client is given a token, that he needs to keep. With this token, he can access to his private storage.
How is composed a token ? Token have several informations, which are the following :
> The IP of the client (online), in order to test that there is no MITM.
> The date of the generation. (Token is valid for one day maximum, from the creation)
> The username of the client, to locate its datas.
> The access name of the service, for the same reason.

<br/>
With the token, client can :

1. **Get a data**
1. **Set a data**
1. **Delete a data**
1. **Delete all its datas for a given service**

A data is stored following this tree : _base directory_ > _datas_ > _store_ > _access name_ > _username_ > _{key}_
Each command can result an error. These errors are defined in the language file. The client can get all error codes, associated with the good language, by requesting the **_errors list_** command. It will result an array with the code and description of all errors. This way, you can display the message you want, or server message directly.
A exhausting table of all error codes can be found in the annexes.
<br/>
##### Interface and Log system
Server interface is made to be user friendly : Easily readable, no useless informations, quickly closable. As soon as the server is launched, configration file is read, and title is displayed. Then, all configurations start, in this order : Language system, Logs, Encryption system, Datas, and Networking.
After system loading, system administrator is able to write his commands. Prompt is displayed with the '#' character. Administrator can print all available commands by typing the "help" command.
All commands result by a print on screen, and display again the prompt character. If a command badly ends, an error message will be displayed.
All successful commands, Local or client, are stored in separatly logs. (as shown in the file structure tree, they are respectively named "localcmd.log" and "socketcmd.log"). If a log file exceeds the maximum lines as indicated in the configuration file, a new log file is created.
<br/>
### Annexes :
Here are some informations you may need to start sucessfully your project, or to have complete knowledge of all you can do with this system.

#### Dotnet core 2.0 installation for _Debian Jessie_
To run the ADOSS server dll, you will need dotnet core 2.0 on your target system, if you want to instal it on a debien, here is the process to achieve it :
```bash
$ sudo apt-get update

$ sudo apt-get install curl libunwind8 gettext

$ curl -sSL -o dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz
$ sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet
$ sudo ln -s /opt/dotnet/dotnet /usr/local/bin

$ dotnet --info
$ rm dotnet.tar.gz
```
<br/>
#### Exhaustive error codes list :
|Code|Description|English message display|
|:--:|----------|-----------------------|
|601|Given command can't be interpreted by the server.|Command does not exist.|
|602|Command exists but given malformed.|Command is malformed.|
|603|Requested user does not exist.|User does not exist.|
|604|Requested service does not exist.|Requested service does not exist.|
|605|Given user does have the access.|Access is denied.|
|606|Given user must wait for access acceptation.|Service is still pending.|
|607|Given password is wrong.|Password incorrect.|
|608|Requested access is granted. User and service token is sent.|Access granted.|
|609|User can only register one time per service.|User already registered.|
|610|Old and New password for a password change are the same.|Password didn't change.|
|611|Password is now updated.|Password successfully changed.|
|612|Requested user is not active. All actions are forbidden.|User is not active.|
|701|User can't be created a second time.|User already exist.|
|702|Service can't be created a second time.|Service already exist.|
|703|Requested service does not exist (server side).|Service does not exist.|
|704|Given user does have access to the requested service.|User does not have this service access.|
|705|User does not have any service access.|User does not have any service access.|
|706|User already have this service access.|User already have this service access.|
|707|Requested log type is not valid.|Log type must be "server" or "client".|
|708|Given parameter is not a number.|Parameter is not a number.|
|709|Given number is probably not a number...|Given number is not valid.|
|801|Token verification failed, because of sender IP...|You are not the token's owner.|
|802|Token lease is over.|Token is not valid anymore.|
|803|Requested data key does not exist.|Data does not exist.|
|804|Requested data key exists. Data is sent.|Data exists.|
|805|Data has been successfully stored.|Data has been stored.|
|806|Data couldn't be stored for some reasons.|Data couldn't be stored.|
|807|Data successfully deleted.|Data has been deleted.|
|808|Data couldn't be deleted for some reasons.|Data couldn't be deleted.|
|809|All datas successfully deleted.|All datas have been deleted.|
|810|All datas couldn't be deleted for some reasons.|All datas couldn't be deleted.|
<br/>
#### Console color possibilities :
    - Black
    - DarkBlue
    - DarkGreen
    - DarkCyan
    - DarkRed
    - DarkMagenta
    - DarkYellow
    - Gray
    - DarkGray
    - Blue
    - Green
    - Cyan
    - Red
    - Magenta
    - Yellow
    - White
    - Black
    - DarkBlue
    - DarkGreen
    - DarkCyan
    - DarkRed
    - DarkMagenta
    - DarkYellow
    - Gray
    - DarkGray
    - Blue
    - Green
    - Cyan
    - Red
    - Magenta
    - Yellow
    - White

<br/>

[If you liked it, give me on paypal ! ;)](https://www.paypal.me/CorentinZ/1)
