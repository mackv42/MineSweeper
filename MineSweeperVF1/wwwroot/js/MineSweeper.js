
//var data;
var flagCharacter = "🚩";
var flag = false;
var data;
document.addEventListener("keyup", function (e) { if (e.key == "f") { flag = flag ? false : true; } });

function createBoardDOM( data ){
	let boardDiv = document.getElementById("board");
	for(let i=0; i<data.length; i++){
		let currentRow = createRow( "r" + i );
		for(let j=0; j<data[i].length; j++){
			let currentCell = createCell(i, j, data[i][j]);
			currentRow.appendChild(currentCell);
		}
		boardDiv.appendChild(currentRow);
	}
}

function clear( id ){
	document.getElementById( id ).innerHTML = "";
}

function createRow( name ){
	let row = document.createElement("div");
	row.classList.add("row");
	row.setAttribute("id", name);
	row.setAttribute("value", name);
	return row;
}

function log( e ){
	console.log(e.value);
}

function newGame(width, height, bombs){
	//ajax here
	//clear board, recreate board on response success
    var ret;
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
           // console.log(Json.Parse(this.responseText));
            clear('board');
            data = JSON.parse(this.responseText);
            createBoardDOM(JSON.parse(this.responseText));
        }
    }
    xhttp.open("GET", "/MineSweeper/GenerateBoard?width="+width+"&height="+height+"&numBombs="+bombs, true)
    xhttp.send();
}

function ClickCell(x, e) {
    let coordinates = x.value.split("-");
    if (flag) {
        if (!data[parseInt(coordinates[0], 10)][parseInt(coordinates[1], 10)].hidden) {
            data[parseInt(coordinates[0], 10)][parseInt(coordinates[1], 10)].flagged = true;
            x.innerHTML = flagCharacter;
            return;
        }
    }
    if (data[parseInt(coordinates[0], 10)][parseInt(coordinates[1], 10)].flagged) {
        data[parseInt(coordinates[0], 10)][parseInt(coordinates[1], 10)].flagged = false;
        x.innerHTML = "<br/>";
        return;
    }

    $.post(
        "/MineSweeper/Click",
        { board: data, x: parseInt(coordinates[1], 10), y:parseInt(coordinates[0], 10) },
        function (d) {
            if (d === undefined) {
                //lose
                console.log("You Lose");
            }
            if (d.length == 0) {
                //win
                console.log("You Win!");
            }

            clear("board");
            data = d;
            createBoardDOM(d);

        }
    );
}

function createCell( row, column, data ){
	let cell = document.createElement("div");
	cell.classList.add("c"+column);
	cell.classList.add("cell");
	let btn = createButton(row, column, data);
	/***change to click request as function when finished***/
	cell.appendChild(createButton(row, column, data, ClickCell));
	return cell; 
}


function createButton(row, column, data, click){
    let btn = document.createElement("BUTTON");
    if (data.flagged) {
        btn.innerHTML = flagCharacter;
    }
    else if (data.hidden) {
        btn.innerHTML = "<br/>";
    }  else {
        btn.innerHTML = data.value;
    }

	btn.setAttribute("value", row+ "-"+column);
	//btn.innerHTML = data;

    btn.addEventListener("click", function () { click(this) })
    //btn.addEventListener("oncontextmenu", function (e) { e.preventDefault(); console.log("hello"); this.innerHTML = flag; return false;}, false)
	
	return btn;
}

//createBoardDOM(board);

//creates a 10X10 board with 15 bombs
//window.onload = newGame(10, 10, 15);
newGame(10, 10, 15);

document.addEventListener('DOMContentLoaded', function () {
    newGame(10, 10, 15);
});