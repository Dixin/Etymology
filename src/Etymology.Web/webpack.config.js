const path = require("path");

const Paths = {
    source: path.join(__dirname, "Client"),
    build: path.join(__dirname, "wwwroot"),
    indexSourceJavaScript: "js/index.js",
    indexBuildJavaScript: "js/[name].[chunkhash].js",
    indexTemplate: "index.ejs",
    indexHTML: "index.htm"
};

module.exports = {
    context: Paths.source,
    entry: path.join(Paths.source, Paths.indexSourceJavaScript),
    output: {
        path: Paths.build,
        filename: Paths.indexBuildJavaScript
    },
    stats: {
        colors: true,
        modules: true,
        reasons: true,
        errorDetails: true
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: [
                    {
                        loader: "babel-loader",
                        options: {
                            "presets": ["env"]
                        }
                    }
                ]
            }
        ]
    },
    Paths: Paths
};
