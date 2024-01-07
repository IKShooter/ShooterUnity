using System;
using System.Collections.Generic;
using Events;
using Network.Enums;
using Network.Models;
using UnityEngine;

namespace Player
{
    public class EntitiesController : MonoBehaviour
    {
        private class RemoteEntity
        {
            public Vector3 pos;
            public EntityModel model;
            public GameObject GameObject;
        }
        
        private EntitiesController _instance;
        public EntitiesController Instance => _instance;

        [SerializeField] private GameObject _entitiesContainer;
        private GameObject _entityAsset;

        private List<RemoteEntity> _remoteEntities;

        public EntitiesController()
        {
            _instance = this;
            _remoteEntities = new List<RemoteEntity>();
        }

        private void Start()
        {
            _entityAsset = Resources.Load<GameObject>("Prefabs/BaseEntity");
            EventsManager<UpdateEntitiesEvent>.Register(OnUpdateEntitiesList);
        }

        private void OnDestroy()
        {
            EventsManager<UpdateEntitiesEvent>.Unregister(OnUpdateEntitiesList);
        }

        private void OnUpdateEntitiesList(List<EntityModel> entities)
        {
            // Debug.Log(entities.Count);
            
            foreach (var entityModel in entities)
            {
                RemoteEntity? remoteEntity = _remoteEntities.Find(e => e.model.Id == entityModel.Id);
                if (remoteEntity != null)
                {
                    remoteEntity.pos = remoteEntity.pos;
                }
                else
                {
                    remoteEntity = new RemoteEntity();
                    remoteEntity.model = entityModel;
                    remoteEntity.GameObject = Instantiate(_entityAsset);

                    switch (remoteEntity.model.EntityType)
                    {
                        case EntityType.Kit:
                            GameObject subGameObject = null;
                            
                            switch (entityModel.KitType)
                            {
                                case KitType.Ammunition:
                                    var asset = Resources.Load<GameObject>("Prefabs/AmmoCrate");
                                    subGameObject = Instantiate(asset);
                                    break;
                                case KitType.Health:
                                    var asset2 = Resources.Load<GameObject>("Prefabs/MedKit");
                                    subGameObject = Instantiate(asset2);
                                    break;
                            }
                            
                            subGameObject.transform.SetParent(remoteEntity.GameObject.transform);
                            subGameObject.transform.localPosition = Vector3.zero;;
                            var boxCollider = subGameObject.GetComponentInChildren<BoxCollider>();
                            if (boxCollider != null)
                                boxCollider.enabled = false;
                            break;
                    }
                    
                    _remoteEntities.Add(remoteEntity);
                    
                    remoteEntity.GameObject.transform.position = remoteEntity.pos;
                    
                    Debug.Log($"Entity {remoteEntity.model.EntityType} created!");
                }
                
                remoteEntity.pos = entityModel.Position;
            }
            
            // Track killed entities
            foreach (RemoteEntity remoteEntity in _remoteEntities)
            {
                // Is leaved
                if (entities.Find(model => model.Id == remoteEntity.model.Id) == null)
                {
                    Debug.Log($"Entity {remoteEntity.model.EntityType} removed!");
                    // TODO: EventsManager<RemotePlayerLeaveEvent>.Trigger?.Invoke(remoteEntity.model);
                    Destroy(remoteEntity.GameObject);
                    _remoteEntities.Remove(remoteEntity);   
                    break;
                }
            }
        }

        private float _interpolationSpeed = 4f;
        
        private void Update()
        {
            foreach (var remoteEntity in _remoteEntities)
            {
                remoteEntity.GameObject.transform.position = Vector3.Lerp(
                    remoteEntity.GameObject.transform.position, remoteEntity.pos,
                    Time.deltaTime * _interpolationSpeed);
                remoteEntity.GameObject.transform.Rotate(Vector3.up, 0.1f);
            }
        }
    }
}