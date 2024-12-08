const fs = require("fs");
const path = require("path");

const webConfig = `<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="Angular Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
`;

const outputPath = path.join(
  __dirname,
  "dist/",
  "web.config"
);

// Ensure the directory exists
fs.mkdir(path.dirname(outputPath), { recursive: true }, (err) => {
  if (err) throw err;
  // Write the web.config file
  fs.writeFile(outputPath, webConfig, (err) => {
    if (err) throw err;
    console.log("web.config has been saved!");
  });
});
