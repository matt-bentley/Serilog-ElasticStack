$Headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$Headers.Add("Content-Type", "application/json")
$Headers.Add("kbn-version", "6.5.4")
Invoke-RestMethod "http://localhost:5601/api/saved_objects/index-pattern" `
      -Method Post `
      -Headers $Headers `
      -Body '{"attributes":{"title":"usage-*","timeFieldName":"@timestamp"}}'
Invoke-RestMethod "http://localhost:5601/api/saved_objects/index-pattern" `
      -Method Post `
      -Headers $Headers `
      -Body '{"attributes":{"title":"diagnostic-*","timeFieldName":"@timestamp"}}'
Invoke-RestMethod "http://localhost:5601/api/saved_objects/index-pattern" `
      -Method Post `
      -Headers $Headers `
      -Body '{"attributes":{"title":"performance-*","timeFieldName":"@timestamp"}}'
Invoke-RestMethod "http://localhost:5601/api/saved_objects/index-pattern" `
      -Method Post `
      -Headers $Headers `
      -Body '{"attributes":{"title":"error-*","timeFieldName":"@timestamp"}}'