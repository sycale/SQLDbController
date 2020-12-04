Data manager, that controls localhost sql server, abilities (insert, delete, view, convert into xml table values). This app can be perfomed as a service by executing following commands:
<code>
sudo mkdir /srv/4lab # Create directory /srv/HelloWorld
sudo chown yourusername /srv/4lab # Assign ownership to yourself of the directory
dotnet publish -c Release -o /srv/4lab
</code>

<b>The published result contains an executable called 'HelloWorld' which will run the application. Let's verify we can also run the published application:</b>

<code>
/srv/4lab/4lab
</code>

<b>
To run services on Linux, Systemd uses 'service unit configuration' files to configure services.
Let's create the file '4lab.service' inside our project so we can store it in source control along with our code. Add the following content to the file:
</b>

<code>
[Unit]
Description=Hello World console application

[Service]

# systemd will run this executable to start the service

ExecStart=/srv/4lab/4lab

# to query logs using journalctl, set a logical name here

SyslogIdentifier=4labLog

# Use your username to keep things simple.

# If you pick a different user, make sure dotnet and all permissions are set correctly to run the app

# To update permissions, use 'chown yourusername -R /srv/4lab' to take ownership of the folder and files,

# Use 'chmod +x /srv/4lab/4lab' to allow execution of the executable file

User=yourusername

# This environment variable is necessary when dotnet isn't loaded for the specified user.

# To figure out this value, run 'env | grep DOTNET_ROOT' when dotnet has been loaded into your shell.

Environment=DOTNET_ROOT=/opt/rh/rh-dotnet31/root/usr/lib64/dotnet

[Install]
WantedBy=multi-user.target
</code>

<b>
Also logs are available at table logs
</b>
