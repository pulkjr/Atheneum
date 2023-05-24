---
type: getting started
author:
- Doe, John
difficulty: In Development
skillset: Any
technology: PowerShell
---
1. On your local computer, open Windows PowerShell, and run the following command:

    ```PowerShell
    $UserCredential = Get-Credential
    ```
    In the Windows PowerShell Credential Request dialog box that opens, enter your user principal name (UPN) (for example, chris@contoso.com) and password, and then click OK.

1. Replace `<ServerFQDN>` with the fully qualified domain name of your Exchange server (for example, mailbox01.contoso.com) and run the following command:

    ```PowerShell
    $Session = New-PSSession -ConfigurationName Microsoft.Exchange -ConnectionUri http://<ServerFQDN>/PowerShell/ -Authentication Kerberos -Credential $UserCredential
    ```
    > Note: The ConnectionUri value is http, not https.

1. import the exchange modules into memory

    ```PowerShell
    Import-PSSession $Session -DisableNameChecking
    ```

    > Warning: Be sure to disconnect the remote PowerShell session when you're finished. If you close the Windows PowerShell window without disconnecting the session, you could use up all the remote PowerShell sessions available to you, and you'll need to wait for the sessions to expire. To disconnect the remote PowerShell session, run the following command:

    ```PowerShell
    Remove-PSSession $Session
    ```