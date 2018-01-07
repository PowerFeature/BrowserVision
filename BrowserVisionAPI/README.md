
BrowserVision is a project that makes a webclient for some of Microsoft's vision services.

It uses the lonekorean's diff-cam-engine for motion detection. To only submit images upon movement. https://github.com/lonekorean/diff-cam-engine

Thanks to David Walsh browser camera article https://davidwalsh.name/browser-camera

Requirements:

Azuresubscription
Customvision Account for customvision training and prediction. Get it at http://customvision.ai
Microsoft Face API for face recognition Get is on at portal.azure.com
Instructions:

Publish the project to an Azure App Service.
In the published App Service Go to Settings -> Application Settings
In Application Settings go to the App Settings section.
Add a new key named "Training-Key" (no quotes) and insert your custom vision training key as value (no dashes).
Add a new key named "Prediction-Key" (no quotes) and insert your custom vision prediction key as value (no dashes).
Add a new key named "FaceApiKey" (no quotes) and insert your FaceApi key as value (no dashes).
Add a new key named "Default-Project-ID" (no quotes) and insert the ID for you default CustomVision Project key as value (no dashes).
Add a new key named "CustomVisionProjectName" (no quotes) and insert the Name for you default CustomVision Project key as value.


Alternativly add the following to the web.config : 

  <appSettings>
    <add key="Training-Key" value="<value here>" />
    <add key="Prediction-Key" value="<value here>" />
    <add key="Default-Project-ID" value="<value here>" />
    <add key="CustomVisionProjectName" value="<value here>" />
    <add key="FaceApiKey" value="<value here>" />
  </appSettings>


Known issues:
Automated training is not in the customvision implementation yet. After training you must go to customvision.ai and train manually. Remember to set the newest iteration to default. Otherwise your new objects will not be recognized. 

Browser issues:
Edge (Browser) somethimes turns the image 90 degrees. This is a browser issue
Only Chrome allows to select from external cameras
Safari on iPhone has known issues. Safari iPad should be ok. 






