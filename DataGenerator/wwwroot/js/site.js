
{
    const baseUrl = "https://localhost:7131";
    const dataTable = document.getElementById("dataTable");
    const mistakesSlider = document.getElementById("errorsCountSlider");
    const mistakesInput = document.getElementById("errorsCountText");
    const regionSelect = document.getElementById("regionInput");
    const table = document.getElementById("dataTable").getElementsByTagName('tbody')[0];
    const seedInput = document.getElementById("seedInput");
    const randomSeed = document.getElementById("randomSeed");

    let currentPage = 1;
    let currentData = [];

    const getRandomInt = (max) => {
        return Math.floor(Math.random() * max);
    }

    function mapGenderToString(gender) {
        return gender === 0 ? "Male" : "Female";
    } 

    const getData = async () => {
        let url = new URL("Home/GetData/", baseUrl);
        url.searchParams.append("region", regionSelect.value);
        url.searchParams.append("mistakesRate", mistakesInput.value);
        url.searchParams.append("pageNumber", currentPage);
        url.searchParams.append("seed", seedInput.value);

        let response = await fetch(url)
            .then(response => response.json());

        response.forEach((item) => {
            console.log(item);
            currentData.push(item);
            let row = table.insertRow(dataTable.rows.length - 1);
            row.insertCell().append(document.createTextNode(currentData.length))
            row.insertCell().appendChild(document.createTextNode(item.id));
            row.insertCell().appendChild(document.createTextNode(item.firstName));
            row.insertCell().appendChild(document.createTextNode(item.lastName));
            row.insertCell().appendChild(document.createTextNode(mapGenderToString(item.gender)));
            row.insertCell().appendChild(document.createTextNode(item.addressString));
            row.insertCell().appendChild(document.createTextNode(item.phoneNumber));
        });

        currentPage++;
    }

    const getCsv = async () => {
        let url = new URL("Home/CreateCsv", baseUrl);

        if (currentData.length > 0) {
            await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify(currentData)
            }).then(response => response.blob())
                .then(blob => {
                    let file = window.URL.createObjectURL(blob);
                    window.location.assign(file);
                });
        }
    }

    handleInputChange = () => {
        document.getElementsByTagName("tbody")[0].innerHTML = "";
        currentPage = 1;
        currentData = [];
        getData();
    }
    

    window.addEventListener('scroll', async () => {
        if ((window.innerHeight + window.scrollY) >= document.documentElement.scrollHeight) {
            await getData();
        }
    });

    window.addEventListener('load', async () => {
        await getData();
    });

    mistakesInput.oninput = (event) => {
        if (Number.isInteger(Number(event.target.value))) {
            if (event.target.value <= 10) {
                mistakesSlider.value = event.target.value;
            }
        } else {
            mistakesSlider.value = 10;
        }

        handleInputChange();
    }

    regionSelect.onchange = () => {
        handleInputChange();
    }

    mistakesSlider.onclick = (event) => {
        mistakesInput.value = event.target.value;
        handleInputChange();
    }

    mistakesInput.oninput = () => {
        if (mistakesInput.value === "") {
            mistakesSlider.value = 0;
        }
        else if (mistakesInput.value <= 10) {
            mistakesSlider.value = mistakesInput.value
        }
        else {
            mistakesSlider.value = 10
        }
        handleInputChange();
    }

    seedInput.oninput = () => {
        handleInputChange();
    }

    randomSeed.onclick = () => {
        seedInput.value = getRandomInt(9999999);
        handleInputChange();
    }

    createCsv.onclick = async () => {
        await getCsv();
    }
}
