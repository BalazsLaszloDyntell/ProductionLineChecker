using NotificationService.Models;

namespace NotificationService.Helpers;

public class EmailUtils
{
    public static string CreateEmailBody(
        DelayIssue delayIssue)
    {
        return $@"
                <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            }}
                            table {{ 
								text-align: left;
								padding-top: 10px;
                            }}
                            th {{
								width: 200px;
								padding-left: 10px;
								background-clip: content-box;
                                background-color: #EEEEEE;
								font-weight: normal;
                            }}
							td {{
								padding: 5px;
								width: 300px;
                                border: 1px solid black;
							}}
							.fine {{
								font-weight: bold;
								color: #FF0000;
							}}
							.logo {{
								float: left;
								display: block;
								margin-top: -15px;
							}}
							.title {{
								display: block;
							}}
							.logo-name {{
								color: #FFFFFF;
								background-color: #2AA3D9;
								vertical-align: middle;
								padding: 10px;
								margin-top: 20px;
								height: 20px;
								width: 400px;
							}}
							.logo-bar {{
								background-color: #005D91;
								width: 420px;
								height: 20px;
								margin-top: -22px;
								margin-bottom: 30px;
							}}
                        </style>
                    </head>
                    <body>
                        <p>Time, {DateTime.Now.ToLongDateString()}</p>
                        
                        <p>Dear Mr. Andrei,</p>
						
						<p>Our system observed a delay in the production line. Below you can find all the details of the delay.</p>

                        <p>
                            <b>Product information:</b>
                            <table>
                                <tr><th>Barcode</th><td>{delayIssue.productId}</td></tr>
                                <tr><th>Product name</th><td>{delayIssue.productName}</td></tr>
                            </table>
                        </p>

                        <p>
                            <b>Delay information:</b>
                            <table>
                                <tr><th>Production line</th><td>{delayIssue.productionLineId}</td></tr>
                                <tr><th>Time at checkpoint 1</th><td>{delayIssue.startTimestamp.ToString("HH:mm:ss")}</td></tr>
                                <tr><th>Time at checkpoint 2</th><td>{delayIssue.endTimestamp.ToString("HH:mm:ss")}</td></tr>
                                <tr><th>Delay</th><td>{delayIssue.delay}s</td></tr>
                            </table>							
                        </p>						
						<p>
						Yours sincerely,<br/>
						Production Line Checker
						</p>
                    </body>
                </html>
            ";
    }
}
