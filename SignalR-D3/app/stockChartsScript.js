var companiesSelector, hub, stockCostData, lineData, chartObj, companies;
$(function () {
    companiesSelector = $("#selCompanies");
    setCompanies().then(function () {
        initiateSignalRConnection();
    });

    hub = $.connection.stockCostsHub;
    setSignalRCallbacks();

    companiesSelector.change(function () {
        hub.server.getInitialStockPrices(companiesSelector.val());
    });
});

function setCompanies() {
    return $.get("api/companies").then(function (data) {
        companies = data;
        var html = "";
        $.each(data, function () {
            html = html + "<option value='" + this.CompanyId + "'>" + this.CompanyName + "</option>";
        });
        companiesSelector.append(html);
    });
}
//
function initiateSignalRConnection() {
    $.connection.hub.start().then(function () {
        hub.server.getInitialStockPrices(companies[0].CompanyId);
    });
}

function setSignalRCallbacks() {
    hub.client.initiateChart = function (stocks) {
        console.log(stocks);
        stockCostData = stocks;
        transformData();
        if (!chartObj) {
            chartObj = initChart();
        }
        else {
            chartObj.redrawChart();
        }
    };

    hub.client.updateNewStockCosts = function (stocks) {
        stockCostData.push(stocks[stockCostData[0].CompanyId - 1]);
        stockCostData.splice(0, 1);
        transformData();
        chartObj.redrawChart();
    };
}

function transformData() {
    lineData = [];
    stockCostData.forEach(function (sc) {
        lineData.push({ cost: sc.Cost, timeAgo: (new Date() - new Date(sc.Time)) / 1000 });
    });
}

function initChart() {

    var svgElement = d3.select("#svgStockChart"),
      width = 1000,
      height = 200,
      padding = 45,
      pathClass = "path";
    var xScale, yScale, xAxisGen, yAxisGen, lineFun;

    drawLineChart();
    
    function setChartParameters() {
        xScale = d3.scale.linear()
                   .range([padding + 5, width - padding])
                   .domain([lineData[0].timeAgo, lineData[lineData.length - 1].timeAgo]);

        yScale = d3.scale.linear()
                   .range([height - padding, 10])
                   .domain([d3.min(lineData, function (d) {
                       return d.cost - 0.2;
                   }),
                    d3.max(lineData, function (d) {
                        return d.cost;
                    })]);

        xAxisGen = d3.svg.axis()
          .scale(xScale)
          .ticks(lineData.length)
          .orient("bottom");

        yAxisGen = d3.svg.axis()
          .scale(yScale)
          .ticks(5)
          .orient("left");

        lineFun = d3.svg.line()
                    .x(function (d) {
                        return xScale(d.timeAgo);
                    })
                    .y(function (d) {
                        return yScale(d.cost);
                    })
                    .interpolate('basis');
    }

    function drawLineChart() {
        setChartParameters();

        svgElement.append("g")
          .attr("class", "x axis")
          .attr("transform", "translate(0," + (height - padding) + ")")
          .call(xAxisGen);

        svgElement.append("text")
            .attr("x", 400)
            .attr("y", 200)
            .style("text-anchor", "middle")
            .text("Seconds ago");

        svgElement.append("g")
          .attr("class", "y axis")
          .attr("transform", "translate(" + padding + ",0)")
          .call(yAxisGen);

        svgElement.append("path")
          .attr({
              "d": lineFun(lineData),
              "stroke": "blue",
              "stroke-width": 2,
              "fill": "none",
              "class": pathClass
          });
    }

    function redrawLineChart() {

        setChartParameters();

        svgElement.selectAll("g.y.axis").call(yAxisGen);

        svgElement.selectAll("g.x.axis").call(xAxisGen);

        svgElement.selectAll("." + pathClass)
            .attr({
                d: lineFun(lineData)
            });
    }

    return {
        redrawChart: redrawLineChart
    }
}
