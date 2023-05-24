---
type: Modify
title: Create a VMWare Snapshot for a Virtual Machine
abstract: Create a VM SnapShot
author: 
- John Doe
difficulty: Professional
disable-publication: true
keywords:
- snapshot
- vm
- vmware
section: Virtual Machine
order: 1
skillset: Operator
technology: Windows
id: 05d54556-bc64-4e7d-a0a1-bafb2bb5a98e
is-impacting: true
verified-metadata: 2023-03-23
verified-content: 2023-03-23
last-trained: 2022-03-23
sme:
- John Doe
- Jane Doe
training-topic: true
training-frequency: SemiAnnually
software-version:
- '6.7'
km-id: KM123456
km-last-sync: 2022-03-23
references:
- title: A KB Article
  link: https://www.google.com
- title: Another
  link: https://www.NetApp.com

---

This article explains how to take a VMWare Virtual Machine (VM) Snapshot using vCenter.

> Warning: Be aware that when creating a VMWare snapshot, disk usage will increase and could lead to the datastore running out of disk space if the snapshot grows too large. Be mindful of this and delete the snapshot as soon as you are sure you will not need to revert the VM.

1. Log into the vCenter where the VM resides.
    ```PowerShell
    Connect-ViServer -Name vCenterServer01
    ```

1. In the **Hosts and Clusters** view, click the VM.
    ```PowerShell
    Import-PSSession $Session -DisableNameChecking
    ```
1. Click the **Actions** dropdown and select **Take Snapshot**.
    ```json
    {
        "test": "something"
    }
    ```

1. In the **Take Snapshot** dialog box, give the snapshot a meaningful name, uncheck the **Snapshot the virtual machine's memory** box and click OK.

  ![TakeSnapshot](../Windows/media/VMSnapshot3.png)

1. Verify that the snapshot was taken in the **Recent Tasks** window.

  ![Verify](../Windows/media/VMSnapshot4.png)