using HWMonitorWebApi.Hubs;
using HWMonitorWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHostedService<HardwareMonitorService>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


app.UseAuthorization();
app.MapControllers();
app.MapHub<HardwareHub>("/hardwarehub");

app.MapGet("/", () => Results.Content(GetHtmlContent(), "text/html"));
app.Urls.Add("http://*:3005");
app.Run();

string GetHtmlContent()
{
    return @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {
            background-color: #121212;
            color: #ffffff;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 20px;
            margin: 0;
        }
        .grid-container {
            display: none;
            grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
            gap: 20px;
            width: 100%;
            max-width: 1200px;
        }
        .card {
            background-color: #1e1e1e;
            border-radius: 10px;
            padding: 15px 0;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
            text-align: center;
            display: flex;
            flex-direction: column;
            justify-content: center;
        }
        .card h2 {
            margin: 0;
            font-size: 15px;
            color: #00ffff;
        }
        .card .value-container {
            display: flex;
            justify-content: center;
            align-items: baseline;
            margin-top: 5px;
        }
        .card .value {
            font-size: 25px;
            font-weight: bold;
            color: #ffffff; /* Static color */
        }
        .card .unit {
            font-size: 18px;
            font-weight: bold;
            color: #ffffff;
            margin-left: 5px;
        }
     
        .dynamic-color {
            color: inherit;
        }       
        #cpu-load .value, #cpu-load .unit,
        #gpu-load .value, #gpu-load .unit,
        #cpu-temp .value, #cpu-temp .unit,
        #gpu-temp .value, #gpu-temp .unit {
            font-size: 50px;
        }
        /* Style for the offline message */
        .offline-message {
            display: none; 
    font-size: 40px;
            font-weight: bold;
    color: #807e7e;
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    text-align: center;
        }
    </style>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/5.0.11/signalr.min.js""></script>
    <script>
        function getColor(value, max) {
            const hue = ((1 - value / max) * 120).toString(10);
            return `hsl(${hue}, 100%, 50%)`;
        }

        document.addEventListener('DOMContentLoaded', function () {
            var connection = new signalR.HubConnectionBuilder()
                .withUrl(""/hardwarehub"")
                .withAutomaticReconnect({
                    nextRetryDelayInMilliseconds: () => 1000
                })
                .build();

            function updateCard(id, value, unit, max = 100, isDynamic = false) {
                const card = document.getElementById(id);
                if(value!=null)
                {
                   card.querySelector('.value').textContent = value;
                }             
                card.querySelector('.unit').textContent = unit;
                if (isDynamic) {
                    card.querySelector('.value').style.color = getColor(value, max);
                } else {
                    card.querySelector('.value').style.color = '#ffffff'; // Static color
                }
            }

            function setConnectionStatus(isConnected) {
                const gridContainer = document.querySelector('.grid-container');
                const offlineMessage = document.querySelector('.offline-message');

                if (isConnected) {
                    gridContainer.style.display = 'grid';
                    offlineMessage.style.display = 'none';
                } else {
                    gridContainer.style.display = 'none';
                    offlineMessage.style.display = 'block';
                }
            }

            connection.onclose(() => {
                setConnectionStatus(false);
            });

            connection.onreconnecting(() => {
                setConnectionStatus(false);
            });

            connection.onreconnected(() => {
                setConnectionStatus(true);
            });

            connection.start()
                .then(() => setConnectionStatus(true))
                .catch(err => {
                    setConnectionStatus(false);
                });

            connection.on(""ReceiveHardwareData"", function (data) {
                updateCard('cpu-load', data.cpuLoad, '%', 100, true);
                updateCard('cpu-temp', data.cpuTemp, '°C', 100, true);
                updateCard('gpu-load', data.gpuLoad, '%', 100, true);
                updateCard('gpu-temp', data.gpuTemp, '°C', 100, true);
                updateCard('gpu-power', data.gpuPower, 'W');
                updateCard('cpu-fan', data.cpuFan, 'RPM');
                updateCard('gpu-fan1', data.gpuFan1, 'RPM');
                updateCard('gpu-fan2', data.gpuFan2, 'RPM');
                updateCard('system-fan1', data.systemFan1, 'RPM');
                updateCard('system-fan2', data.systemFan2, 'RPM');
                updateCard('ram', data.ram, 'GB');
                updateCard('gpu-memory', data.gram, 'GB');
            });
        });
    </script>
</head>
<body>
    <div class=""offline-message"">OFFLINE</div>
    <div class=""grid-container"">
        <div class=""card"" id=""cpu-load"">
            <h2>CPU Load</h2>
            <div class=""value-container"">
                <span class=""value dynamic-color"">--</span><span class=""unit"">%</span>
            </div>
        </div>
        <div class=""card"" id=""cpu-temp"">
            <h2>CPU Temperature</h2>
            <div class=""value-container"">
                <span class=""value dynamic-color"">--</span><span class=""unit"">°C</span>
            </div>
        </div>
        <div class=""card"" id=""gpu-load"">
            <h2>GPU Load</h2>
            <div class=""value-container"">
                <span class=""value dynamic-color"">--</span><span class=""unit"">%</span>
            </div>
        </div>
        <div class=""card"" id=""gpu-temp"">
            <h2>GPU Temperature</h2>
            <div class=""value-container"">
                <span class=""value dynamic-color"">--</span><span class=""unit"">°C</span>
            </div>
        </div>
        <div class=""card"" id=""ram"">
            <h2>RAM</h2>
            <div class=""value-container"">
                <span class=""value"">--</span><span class=""unit"">GB</span>
            </div>
        </div>       
        <div class=""card"" id=""cpu-fan"">
            <h2>CPU Fan</h2>
            <div class=""value-container"">
                <span class=""value"">--</span><span class=""unit"">RPM</span>
            </div>
        </div>
         <div class=""card"" id=""system-fan1"">
            <h2>System Fan 1</h2>
            <div class=""value-container"">
                <span class=""value"">--</span><span class=""unit"">RPM</span>
            </div>
        </div>
        <div class=""card"" id=""system-fan2"">
            <h2>System Fan 2</h2>
            <div class=""value-container"">
                <span class=""value"">--</span><span class=""unit"">RPM</span>
            </div>
        </div>
        <div class=""card"" id=""gpu-memory"">
            <h2>GPU Memory</h2>
            <div class=""value-container"">
                <span class=""value"">--</span><span class=""unit"">GB</span>
            </div>
        </div>
        <div class=""card"" id=""gpu-power"">
            <h2>GPU Power</h2>
            <div class=""value-container"">
                <span class=""value"">--</span><span class=""unit"">W</span>
            </div>
        </div>
        <div class=""card"" id=""gpu-fan1"">
            <h2>GPU Fan 1</h2>
            <div class=""value-container"">
                <span class=""value"">--</span><span class=""unit"">RPM</span>
            </div>
        </div>
        <div class=""card"" id=""gpu-fan2"">
            <h2>GPU Fan 2</h2>
            <div class=""value-container"">
                <span class=""value"">--</span><span class=""unit"">RPM</span>
            </div>
        </div> 
    </div>
</body>
</html>";
}





