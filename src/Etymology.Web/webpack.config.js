const path = require("path");

const Paths = {
    source: path.join(__dirname, "Client"),
    build: path.join(__dirname, "wwwroot"),
    indexJavaScript: "js/index.js",
    indexTemplate: "index.ejs",
    indexHTML: "index.htm"
};

module.exports = {
    context: Paths.source,
    entry: path.join(Paths.source, Paths.indexJavaScript),
    output: {
        path: path.join(Paths.build),
        filename: Paths.indexJavaScript
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
