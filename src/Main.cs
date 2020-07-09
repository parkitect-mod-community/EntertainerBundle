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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Parkitilities;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
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

            SpriteRenderer pandaSprite = AssetPackUtilities.LoadAsset<GameObject>(_assetBundle, "c08cc832b55af5f638a6f8c64f6258fb")
                .GetComponent<SpriteRenderer>();
            Parkitility.CostumeBuilder()
                .Id("EntertainerPanda-cc65c162")
                .DisplayName("Panda")
                .GuestThoughtAboutCostume("What a cute fluffy Panda!")
                .CostumeSprite("panda", Object.Instantiate(pandaSprite.sprite),50,50)
                .BodyPartMale(
                    Parkitility.CreateBodyPart()
                        .AddTorso(_remap(bodyPartsContainer.getTorso(0),
                            AssetPackUtilities.LoadAsset<GameObject>(_assetBundle, "38c9bccac03f19b6caa53a4c4f656334")))
                        .AddHairstyle(_remapHead(AssetPackUtilities.LoadAsset<GameObject>(_assetBundle,
                            "595f9e3b779740751893aa986ff5ad87")))
                        .Build(_assetManagerLoader))
                .MeshAnimations(raptorCostume.meshAnimations)
                .AnimatorController(raptorCostume.animatorController)
                .Register(_assetManagerLoader, entertainers);

            SpriteRenderer tigerSprite = AssetPackUtilities.LoadAsset<GameObject>(_assetBundle, "8d3a87427a6819f9e9c457c3feef4a56")
                .GetComponent<SpriteRenderer>();
            Parkitility.CostumeBuilder()
                .Id("EntertainerTiger-cc65c162")
                .DisplayName("Tiger")
                .GuestThoughtAboutCostume("What a fluffy Tiger!")
                .CostumeSprite("tiger", Object.Instantiate(tigerSprite.sprite),50,50)
                .BodyPartMale(
                    Parkitility.CreateBodyPart()
                        .AddTorso(_remap(bodyPartsContainer.getTorso(0),
                            AssetPackUtilities.LoadAsset<GameObject>(_assetBundle, "21d83d7233511714f875e57875992cee")))
                        .AddHairstyle(_remapHead(AssetPackUtilities.LoadAsset<GameObject>(_assetBundle,
                            "20bc030acb52b1cb09ada2ed8131cef0")))
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
