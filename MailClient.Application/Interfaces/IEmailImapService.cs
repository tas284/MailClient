﻿using MailClient.Application.InputModel;

namespace MailClient.Application.Interfaces
{
    public interface IEmailImapService
    {
        string SyncMessages(SyncEmailImapInputModel input);
    }
}
