C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe pbr.vert -o pbr.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe pbr.frag -o pbr.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe line.vert -o line.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe line.frag -o line.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe wireframe.vert -o wireframe.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe wireframe.frag -o wireframe.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe celShaded.vert -o celShaded.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe celShaded.frag -o celShaded.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe screenquad.vert -o screenquad.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe screenquad.frag -o screenquad.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe particle.vert -o particle.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe particle.frag -o particle.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe ui.vert -o ui.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe ui.frag -o ui.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe uiText.vert -o uiText.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe uiText.frag -o uiText.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe shadowOffscreen.vert -o shadowOffscreen.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe shadowOffscreen.frag -o shadowOffscreen.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe bloomBlend.vert -o bloomBlend.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe bloomBlend.frag -o bloomBlend.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe bloomColor.vert -o bloomColor.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe bloomColor.frag -o bloomColor.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe gaussian.vert -o gaussian.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe gaussian.frag -o gaussian.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe water.vert -o water.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe water.frag -o water.frag.spv

C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe skybox.vert -o skybox.vert.spv
C:/VulkanSDK/1.2.148.1/Bin32/glslc.exe skybox.frag -o skybox.frag.spv
pause


##@echo off
##for /r %%i in (*.frag, *.vert) do %VULKAN_SDK%/Bin/glslangValidator.exe -V %%i