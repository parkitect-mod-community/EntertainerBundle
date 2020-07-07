/**
* Copyright 2019 Some Indvidual
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Linq;
using Parkitilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EntertainerBundle
{
    public class Main : AssetMod
    {
        private AssetManagerLoader _assetManagerLoader = new AssetManagerLoader();
        private AssetBundle _assetBundle;

        private GameObject _remap(GameObject target, GameObject from)
        {
            GameObject go = Object.Instantiate(target);
            AssetPackUtilities.RemapSkinnedMesh(go, from);
            foreach (Renderer componentsInChild in go.GetComponentsInChildren<Renderer>())
            {
                Parkitility.ReplaceWithParkitectMaterial(componentsInChild);
            }

            _assetManagerLoader.HideGo(go);
            return go;
        }

        private GameObject _remapHead(GameObject target)
        {
            GameObject go = Object.Instantiate(target);
            foreach (Renderer componentsInChild in go.GetComponentsInChildren<Renderer>())
            {
                Parkitility.ReplaceWithParkitectMaterial(componentsInChild);
            }
            _assetManagerLoader.HideGo(go);
            return go;
        }

        public override void onEnabled()
        {

            if (GameController.Instance != null && GameController.Instance.isCampaignScenario)
                return;

            Debug.Log(System.IO.Path.Combine(Parkitility.CurrentModDirectory(), "assetpack"));
            _assetBundle =
                AssetBundle.LoadFromFile(System.IO.Path.Combine(Parkitility.CurrentModDirectory(), "assetpack"));
            if (_assetBundle == null)
                throw new Exception("Failed to load AssetBundle!");

            var entertainers = AssetManager.Instance.getPrefab<Entertainer>(Prefabs.Entertainer);

            EmployeeCostume raptorCostume = entertainers.costumes.First(k => k.name == "EntertainerCostumeRaptor");
            var bodyPartsContainer = raptorCostume.bodyPartsMale;

            Parkitility.CostumeBuilder()
                .Id("EntertainerPanda-cc65c162")
                .DisplayName("Panda")
                .BodyPartMale(
                    Parkitility.CreateBodyPart()
                        .AddTorso(_remap(bodyPartsContainer.getTorso(0),
                            AssetPackUtilities.LoadAsset<GameObject>(_assetBundle, "38c9bccac03f19b6caa53a4c4f656334")))
                        .AddHairstyle(_remapHead(AssetPackUtilities.LoadAsset<GameObject>(_assetBundle, "595f9e3b779740751893aa986ff5ad87")))
                        .Build(_assetManagerLoader))
                .MeshAnimations(raptorCostume.meshAnimations)
                .AnimatorController(raptorCostume.animatorController)
                .Register(_assetManagerLoader, entertainers);
            _assetBundle.Unload(false);

        }

        public override void onDisabled()
        {
            if (_assetBundle != null)
                _assetBundle.Unload(false);

            if (_assetManagerLoader != null)
                _assetManagerLoader.Unload();
        }
    }
}
