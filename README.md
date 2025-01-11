# Nexus-Clearing
Nexus Clearing is a server application for handling GDPR deletion
from requests and managing DataStoreService deletions.

## Setup
### Hosting
As part of the [Roblox webhooks documentation](https://create.roblox.com/docs/reference/cloud/webhook-notifications#setting-up-webhook-urls),
Nexus Clearing must be publicly accessible and using HTTPS. This means
that the server application must have an SSL certificate. At the moment,
Nexus Clearing is unable to accept SSL certificates. Using nginx as
a reverse proxy is recommended to accept traffic on port 443 and
redirect it to the needed port (8000 for the app directly, or 8000 by
default in the Docker compose).

### Configuration
Currently, there is no configuration beyond the database file path
and anything managed through `docker-compose.yml`.

### Server Application
The application can be compiled and run using .NET 9. It is highly
recommended to use Docker if possible for running. Using the
`docker-compose.yml` on a system set up with Docker, the server can
be built (or updated) and started with the following when run in the
root directory of the project.

```bash
docker compose up -d --build
```

Stopping the server is done with the following:
```bash
docker compose down
```

### Database
Nexus Clearing currently only supports SQLite, which is created after
the first run of Nexus Clearing. In order to process requests, 2 database
tables must be populated using whichever tool you prefer (none is included):
- `DataStores`
  - `Id`: Automatically generated id for the database. No value needs to be
    manually set.
  - `GameId`: Id of the game (NOT place) that contains the DataStore.
  - `DisplayName`: Arbitrary display name of the game. For now, this is unused
    and is functionally a comment.
  - `DataStoreName`: Name of the DataStore to clear when there is a request.
    - To replace with a user id, use `{UserId}`. For example, `{UserId}_SaveData`
      for user 12345 will use `12345_SaveData` as the DataStore name.
  - `DataStoreKey`: Name of the key in the DataStore to clear when there is
    a request.
    - To replace with a user id, use `{UserId}`. For example, `{UserId}_SaveData`
      for user 12345 will use `12345_SaveData` as the DataStore key.
- `RobloxGameKeys`
  - `GameId`: Id of the game (NOT place) that contains the DataStore.
  - `WebHookSecret`: Secret text the webhook is configured to use.
  - `OpenCloudApiKey`: Open Cloud API key for deleting data in DataStores.
    Only delete permissions are required, and it should be locked down to
    only the DataStores that can be cleared. A CIDR for only the host is
    strongly recommended as well.

### Webhook
In the [webhooks settings](https://create.roblox.com/dashboard/settings/webhooks),
a webhook needs to be created that will trigger for Right to Erasure Requests.
It must include a secret that matches what the database has and the URL must
use `https://YOUR_HOST_HERE/clearing/roblox` with `YOUR_HOST_HERE` being the
hosting. For `thenexusavenger.io`, which uses `clearing.thenexusavenger.io`,
the URL becomes `https://clearing.thenexusavenger.io/clearing/roblox`.

Once the webhook is set up (doesn't need to be created), "Test Response" can
be used to verify the connectivity. In the logs, `Got request for SampleNotification instead of RightToErasureRequest.` should appear once this is done. When running with
actual notifications, `Request had invalid signature.` in the logs means that
the signature could not be verified. Either the secrets in the database don't
match or it isn't an actual request from Roblox.

### Verification
When a request comes in to clear data, there will be 1 of 2 log messages:
- `Request to delete user USER_ID was already queued.` - The request was
  already accepted but isn't cleared yet.
- `Adding deletion request for user USER_ID for game ids GAME_IDS` - The
  request was added to the queue.

The ids for the second log message will appear in the `RobloxUsers` table.
Every 15 minutes (not configurable), the queue will be cleared in the
background with each user having 1 of several messages:
- `Clearing data for USER_ID` - Clearing has started for a user.
- `User USER_ID was cleared.` - Clearing has been completed for a user.
- `User USER_ID was meant to be cleared for GAME_ID but has no DataStore keys.` -
  A game id was sent by Roblox to be cleared but has no DataStore keys to
  clear. This is normal if you have a game with no DataStores, but should
  not appear if you expect DataStores for the game id.
- `GAME_ID has no configured Open Cloud API key.` - DataStores clearing was
  attempted but was not attempted due to having no Open Cloud API key set.
  **This will cause the user to go into a retry state and must be corrected.**
- `An exception occurred trying to clear user USER_ID` - An error occurred
  trying to clear a user, such as Roblox being down or the Open Cloud API key
  not being configured correctly to delete keys. **This will cause the user to
  go into a retry state. Permission issues must be corrected,** while Roblox
  issues can be ignored.

# License
Nexus Clearing is available under the terms of the GNU Lesser General Public
License. See [LICENSE](LICENSE) for details.