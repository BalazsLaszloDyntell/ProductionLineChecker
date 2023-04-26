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
  td2.textContent = prod?.ProcudtName;
  var td3 = document.createElement("td");
  td3.textContent = prod?.ProdLine;
  var td4 = document.createElement("td");
  td4.textContent = prod?.Checkpoint;
  var td5 = document.createElement("td");
  td5.textContent = new Date(prod?.Timestamp).toLocaleDateString('ro-RO', {
    hour: "numeric",
    minute: "numeric",
    second: "numeric"
  });

  tr.appendChild(td1);
  tr.appendChild(td2);
  tr.appendChild(td3);
  tr.appendChild(td4);
  tr.appendChild(td5);

  table.insertBefore(tr, table.children[1]);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", async function (event) {
  connection.invoke("StartProd").catch(function (err) {
    return console.error(err.toString());
  });
});