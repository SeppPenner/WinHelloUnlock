﻿WinHelloUnlock: Unlock KeePass 2 Databases with Windows Hello
=============================================
[![Latest release](https://img.shields.io/github/release/Angelelz/WinHelloUnlock.svg?label=latest%20release)](https://github.com/Angelelz/WinHelloUnlock/releases/latest)
[![Github All Releases](https://img.shields.io/github/downloads/Angelelz/WinHelloUnlock/total.svg)](https://github.com/Angelelz/WinHelloUnlock/releases)
[![License](https://img.shields.io/github/license/Angelelz/WinHelloUnlock.svg)](https://github.com/Angelelz/WinHelloUnlock/blob/master/LICENSE)

This plugin for [KeePass 2][KeePass] password manager is intended to conveniently Unlock your database using biometrics with [Windows Hello technology][WinHello].

[KeePass]: https://keepass.info/
[WinHello]: https://support.microsoft.com/en-us/help/17215/windows-10-what-is-hello

This plugin is heavily based on [KeePassWinHello][KeePassWinHello] and [KeePassQuickUnlock][QuickUnlock]. I am not a programmer, so I copied most of the code from them, but implemented a different way of storing the MasterKey data to allow plugin functionality, even after rebooting your computer, using Password Vault, Password Credentials and Key Credentials from Windows UWP APIs.

[KeePassWinHello]: https://github.com/sirAndros/KeePassWinHello
[QuickUnlock]: https://github.com/JanisEst/KeePassQuickUnlock

Disclaimer
-----

I tried my best to not compromise security! Please, take a look at the code and tell me what could be better. Having said that, we know nothing about Windows Hello internals, and how secure it actually is. By using this plugin, you are putting your trust in my implementation of Windows Hello technology (that you can check on the code), and Windows Hello robustness itself (that you cannot check).

Usage
-----

With this plugin you may:

Unlock your database using Biometric via Windows Hello. Even after completely closing KeePass or rebooting your PC.

Systems Requirements
--------------------

This plugin relies on Windows Hello API and its [requirements][WinHelloReq].

Tested on HP Spectre x360 with KeePass 2.42.1.

[WinHelloReq]: https://www.microsoft.com/en-US/windows/windows-10-specifications

How to Install
--------------

Place [WinHelloUnlock.dll][binLink] into `Plugins` folder in your KeePass installation
*(by default is `C:\Program Files (x86)\KeePass Password Safe 2`)*.

[binLink]: https://github.com/Angelelz/WinHelloUnlock/releases "Plugin Releases"

Setup
-----

After installation, open your database and unlock it using your composite key. Unlocking with any combination of Password/KeyFile/WindowsUserAccount is supported. Secure Desktop is supported.

<img src="https://raw.githubusercontent.com/Angelelz/WinHelloUnlock/master/WinHelloUnlock/Screenshots/ToUnlock.png" width=770/>

When your database is unlocked, you will be asked if you want to set up WinHelloUnlock.

<img src="https://raw.githubusercontent.com/Angelelz/WinHelloUnlock/master/WinHelloUnlock/Screenshots/FirstPrompt.png" width=381/>

A Windows Hello prompt will be shown to cryptographically sign and encrypt your Master Key data.

<img src="https://raw.githubusercontent.com/Angelelz/WinHelloUnlock/master/WinHelloUnlock/Screenshots/WinHello.png" width=449/>

You should receive a confirmation after a successful set up.

<img src="https://raw.githubusercontent.com/Angelelz/WinHelloUnlock/master/WinHelloUnlock/Screenshots/Confirmation.png" width=258/>

Options
-------

The plugin integrates itself into the KeePass settings dialog.

<img src="https://raw.githubusercontent.com/Angelelz/WinHelloUnlock/master/WinHelloUnlock/Screenshots/Options.png" width=600/>

Available settings:

* Enable or disable the plugin for this particular database. If you disable it, you will not be asked to set WinHelloUnlock every time you unlock your database.
* Create or delete WinHelloUnlock data for this particular database.

Notes
-----

No sensitive information including master passwords for databases are stored by the plugin in a plain text. A database key is encrypted and decrypted using Windows Hello API in order to unlock the database.
KeePass Composite Key data is [*Encrypted*](https://docs.microsoft.com/en-us/uwp/api/windows.security.cryptography.core.cryptographicengine.encrypt) with a [*Cryptographic Key*](https://docs.microsoft.com/en-us/uwp/api/windows.security.cryptography.core.cryptographickey) signed with a *Windows Hello* [*Key Credential*](https://docs.microsoft.com/en-us/uwp/api/windows.security.credentials.keycredential) and saved as a [*Password Credential*](https://docs.microsoft.com/en-us/uwp/api/windows.security.credentials.passwordcredential) to a [*Password Vault*](https://docs.microsoft.com/en-us/uwp/api/windows.security.credentials.passwordvault).
To decrypt this data, you need to:
* Have access to the Password Vault
* Have access to the Cryptographic Key
* Be able to Cryptographically sign the Cryptographic Key with Windows Hello

So basically, you need to have access to the data, the hardware and the biometrics (or pin).

As I said, I'm not a programmer, so any criticism will be well received. Feel free to commit any change or PR.

Credits
-------

* _Microsoft_ for [Windows Hello][WinHello] technology
* _JanisEst_ and his [KeePassQuickUnlock](https://github.com/JanisEst/KeePassQuickUnlock)
* _sirAndros_ and his [KeePassWinHello](https://github.com/sirAndros/KeePassWinHello)

WinHelloUnlock vs KeePassWinHello
-------

By the time this plugin was created, KeePassWinHello did not have to option to remain active after Keepass is completly closed, so that was the main reason for it to be crated in the first place. I think they were working on that option, but I could not help them beacuse I did not understand most of their code (Way too advanced implementation for a beginner like me). I think they save the MasterKey info in memory, but WinHelloUnlock saves it encrypted to a Windows Password Credential.

WinHelloUnlock does not implement a way for the credential to expire (like KeePassWinHello do), but implements a way for the credential to be deleted by the user.
