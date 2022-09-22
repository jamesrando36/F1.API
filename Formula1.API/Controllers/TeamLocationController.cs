﻿using Formula1.API.DataStore;
using Formula1.API.Models.TeamLocation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Formula1.API.Controllers
{
    [Route("api/teams/{teamId}/location")]
    [ApiController]
    public class TeamLocationController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<TeamLocationDto>> GetTeamLocations(int teamId)
        {
            var teams = TeamsDataStore.GetData.Teams.FirstOrDefault(t => t.Id == teamId);

            if (teams == null)
            {
                return NotFound();
            }

            return Ok(teams.TeamLocations);
        }

        [HttpGet("{locationId}")]
        public ActionResult<IEnumerable<TeamLocationDto>> GetTeamLocation(int teamId, int locationId)
        {
            var teams = TeamsDataStore.GetData.Teams.FirstOrDefault(t => t.Id == teamId);

            if (teams == null)
            {
                return NotFound();
            }

            var teamLocation = teams.TeamLocations.FirstOrDefault(t => t.Id == locationId);

            if (teamLocation == null)
            {
                return NotFound();
            }

            return Ok(teamLocation);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TeamLocationDto))]
        public ActionResult<TeamLocationDto> CreateTeamLocation(
            int teamId, TeamLocationCreationDto teamLocationCreationDto)
        {

            var team = TeamsDataStore.GetData.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null)
            {
                return NotFound();
            }

            var maxTeamLocationsId = TeamsDataStore.GetData.Teams.SelectMany(
                t => t.TeamLocations).Max(t => t.Id);

            // will be using automapper instead of this
            var newTeamLocation = new TeamLocationDto()
            {
                Id = maxTeamLocationsId,
                Description = teamLocationCreationDto.Description,
                Location = teamLocationCreationDto.Location

            };

            team.TeamLocations.Add(newTeamLocation);

            return newTeamLocation;
        }

        [HttpPut("{locationId}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TeamLocationDto))]
        public ActionResult UpdateTeamLocation(
            int teamId, int locationId, TeamLocationUpdateDto teamLocationPutDto)
        {
            var team = TeamsDataStore.GetData.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null)
            {
                return NotFound();
            }

            var teamLocationFromStore = team.TeamLocations.FirstOrDefault(t => t.Id == locationId);

            if (teamLocationFromStore == null)
            {
                return NotFound();
            }

            teamLocationFromStore.Location = teamLocationPutDto.Location;
            teamLocationFromStore.Description = teamLocationPutDto.Description;

            return NoContent();
        }

        [HttpPatch("{locationId}")]
        public ActionResult PartialUpdateTeamLocation(
            int teamId, int locationId,
            JsonPatchDocument<TeamLocationUpdateDto> patchDocument)
        {
            var team = TeamsDataStore.GetData.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null)
            {
                return NotFound();
            }

            var teamLocationFromStore = team.TeamLocations.FirstOrDefault(t => t.Id == locationId);

            if (teamLocationFromStore == null)
            {
                return NotFound();
            }

            var teamLocationsToPatch =
                new TeamLocationUpdateDto()
                {
                    Location = teamLocationFromStore.Location,
                    Description = teamLocationFromStore.Description
                };

            patchDocument.ApplyTo(teamLocationsToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            teamLocationFromStore.Location = teamLocationsToPatch.Location;
            teamLocationFromStore.Description = teamLocationsToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{locationId}")]
        public ActionResult DeleteTeamLocation( int teamId, int locationId)
        {
            var team = TeamsDataStore.GetData.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team == null)
            {
                return NotFound();
            }

            var teamLocationFromStore = team.TeamLocations.FirstOrDefault(t => t.Id == locationId);
            if (teamLocationFromStore == null)
            {
                return NotFound();
            }

            team.TeamLocations.Remove(teamLocationFromStore);
            return NoContent();
        }
    }
}