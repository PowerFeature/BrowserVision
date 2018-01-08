# BrowserVision
### BrowserVision is a web client PoC for some of Microsoft's vision services.

It uses the lonekorean's diff-cam-engine for motion detection. To only submit images upon movement. https://github.com/lonekorean/diff-cam-engine
Thanks to David Walsh browser camera article https://davidwalsh.name/browser-camera

### Requirements:
- Azure subscription
- Customvision Account for customvision training and prediction. Get it at http://customvision.ai account keys can be found at https://customvision.ai/projects#/settings
- Microsoft Face API for face recognition Get is on at portal.azure.com

### Instructions:
1. Register the required accounts for customvision, and face Api
2. Create a project for your customvision account at https://customvision.ai/projects 
3. Publish the project to an Azure App Service.
4. In the published App Service Go to Settings -> Application Settings
5. In Application Settings go to the App Settings section.
6. Add a new key named "Training-Key" (no quotes) and insert your custom vision training key as value (no dashes) For Customvision Training.
7. Add a new key named "Prediction-Key" (no quotes) and insert your custom vision prediction key as value (no dashes) For Customvision prediction.
8. Add a new key named "CustomVisionProjectName" (no quotes) and insert the Name for the  CustomVision Project you have created in step 2 - e.g. MyCoolDemo this is used for both training and prediction.
9. Add a new key named "FaceApiKey" (no quotes) and insert your FaceApi key as value (no dashes) For Face API demo.


Alternativly add the following to the web.config : 
```
  <appSettings>
    <add key="Training-Key" value="<Customvision trainingkey " />
    <add key="Prediction-Key" value="<value here>" />
    <add key="CustomVisionProjectName" value="<value here>" />
    <add key="FaceApiKey" value="<value here>" />
  </appSettings>
```
## Known issues:
- The backend has some warm-up time. This leads to the first image processed is slow, sometimes a browser refresh is needed. 
- Automated training is not in the customvision implementation yet. After training you must go to customvision.ai and train manually. Remember to set the newest iteration to default. Otherwise your new objects will not be recognized. 
- The client side is intentionally very thin on the CustomVision side. The backend holds and handles account keys and project names. As a consequence it has a lot of redundant calls on the customvision side for identifying projects and downloading tags. A Customvision training request places about 4 calls to customvision - this is intentional but not optimal :-)

### Browser issues:
- Edge (Browser) somethimes turns the image 90 degrees. This is a browser issue
- Only Chrome allows to select from external cameras
- Safari on iPhone has known issues. User controls has been added https://github.com/webrtc/samples/issues/929 . Safari iPad should be ok. 
