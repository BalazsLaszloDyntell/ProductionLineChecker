"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/startprod").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("StartProd", function (_prod) {
  console.log(_prod);
  let prod = JSON.parse(_prod);
  var table = document.getElementById("prodList");
  var tr = document.createElement("tr");

  var td1 = document.createElement("td");
  td1.textContent = prod?.Barcode;
  var td2 = document.createElement("td");
  td2.textContent = prod?.Name;
  var td3 = document.createElement("td");
  td3.textContent = prod?.ProductionLineId;

  tr.appendChild(td1);
  tr.appendChild(td2);
  tr.appendChild(td3);

  table.insertBefore(tr, table.children[1]);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", async function (event) {
  let count = 0;
  while(count < 100) {
    await new Promise((resolve) => {
      setTimeout(() => {
        connection.invoke("StartProd").catch(function (err) {
          return console.error(err.toString());
        });
        resolve();
      }, 1000)
    });
    count++;
  }
});