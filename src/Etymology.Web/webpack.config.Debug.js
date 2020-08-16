const webpack = require("webpack");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const HtmlWebpackInlineSourcePlugin = require("html-webpack-inline-source-plugin");

const { merge } = require('webpack-merge');
const common = require("./webpack.config.js");

const Paths = common.Paths;
delete common.Paths;

module.exports = merge(common, {
    mode: "development",
    devtool: "eval-source-map",
    plugins: [
        new HtmlWebpackPlugin({
            template: Paths.indexTemplate,
            filename: Paths.indexHTML,
            inject: "body",
            showErrors: true
        }),
        new webpack.ProvidePlugin({
            jQuery: "jquery"
        })
    ],
    optimization: {
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /[\\/]node_modules[\\/]/,
                    name: 'vendors',
                    chunks: 'all',
                },
            },
        }
    }
});
